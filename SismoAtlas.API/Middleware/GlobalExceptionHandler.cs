using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SismoAtlas.API.Services;

namespace SismoAtlas.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                CountryApiException ex => ((int)ex.StatusCode, "Error al consultar la información del país", ex.Message),
                OperationCanceledException when httpContext.RequestAborted.IsCancellationRequested
                    => (499, "Solicitud cancelada", "La solicitud fue cancelada."),
                TaskCanceledException => ((int)HttpStatusCode.GatewayTimeout, "Tiempo de espera agotado",
                    "El servicio externo tardó demasiado en responder. Intente de nuevo."),
                HttpRequestException => ((int)HttpStatusCode.BadGateway, "Servicio externo no disponible",
                    "No se pudo conectar con el servicio externo. Intente de nuevo más tarde."),
                JsonException => ((int)HttpStatusCode.BadGateway, "Respuesta inesperada",
                    "El servicio externo devolvió datos que no se pudieron leer."),
                _ => ((int)HttpStatusCode.InternalServerError, "Error interno",
                    "Ocurrió un error inesperado. Intente de nuevo más tarde.")
            };

            if (status >= 500)
                _logger.LogError(exception, "Error no controlado en {Path}", httpContext.Request.Path);
            else
                _logger.LogWarning("Solicitud con error {Status} en {Path}: {Message}", status, httpContext.Request.Path, exception.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problem, (JsonSerializerOptions?)null, "application/problem+json", cancellationToken);

            return true;
        }
    }
}