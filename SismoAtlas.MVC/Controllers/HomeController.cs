
using Microsoft.AspNetCore.Mvc;
using SismoAtlas.MVC.Services;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SismoAtlas.MVC.Controllers
{
    // Controller de la única pantalla de la aplicación.
    // Maneja las búsquedas, validaciones y errores.
    public class HomeController : Controller
    {
        private readonly SismoAtlasApiService _apiService;

        // Recibir el servicio registrado en Program.cs.
        public HomeController(SismoAtlasApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? pais)
        {
            // Si es la primera vez que abre la página.
            if (!Request.Query.ContainsKey("pais"))
            {
                return View();
            }

            // Validar que no esté vacío.
            if (string.IsNullOrWhiteSpace(pais))
            {
                ViewBag.Error = "Debes escribir el nombre de un país.";
                return View();
            }

            // Eliminar espacios al inicio y al final.
            pais = pais.Trim();

            // Validar longitud máxima.
            if (pais.Length > 100)
            {
                ViewBag.Error = "El nombre no puede superar los 100 caracteres.";
                return View();
            }

            // Validar caracteres permitidos.
            if (!Regex.IsMatch(pais, @"^[\p{L}\s.'-]+$"))
            {
                ViewBag.Error = "El nombre del país contiene caracteres no permitidos.";
                return View();
            }

            // Conservar el nombre del país en el buscador.
            ViewBag.PaisBuscado = pais;

            // Consultar la API y manejar posibles errores.
            try
            {
                // Solicitar los terremotos y la información del país.
                var resultado = await _apiService.BuscarPorPaisAsync(
                    pais, HttpContext.RequestAborted);

                // Verificar que la API haya devuelto información.
                if (resultado == null)
                {
                    ViewBag.Error = "No se recibió información del país solicitado.";
                    return View();
                }

                // Guardar la respuesta para mostrarla en la vista.
                ViewBag.Resultado = resultado;

                return View();
            }
            catch (HttpRequestException ex)
            {
                // Manejar errores devueltos por la API.
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "No se encontró información para el país solicitado.";
                }
                else if (ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.Error = "La API rechazó el nombre del país ingresado.";
                }
                else if (ex.StatusCode == null)
                {
                    ViewBag.Error = "No fue posible conectar con la API de SismoAtlas.";
                }
                else
                {
                    ViewBag.Error = "La API presentó un problema al procesar la búsqueda.";
                }

                return View();
            }
            catch (TaskCanceledException) when (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Manejar solicitudes que superen el tiempo permitido.
                ViewBag.Error = "La consulta tardó demasiado. Inténtalo nuevamente.";
                return View();
            }
            catch (JsonException)
            {
                // Manejar respuestas JSON incorrectas.
                ViewBag.Error = "La API devolvió información que no se pudo procesar.";
                return View();
            }
            catch (Exception)
            {
                // Manejar cualquier otro error inesperado.
                ViewBag.Error = "Ocurrió un error inesperado al realizar la búsqueda.";
                return View();
            }
        }
    }
}
