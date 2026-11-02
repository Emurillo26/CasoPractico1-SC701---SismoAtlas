
using System.Net.Http.Json;
using SismoAtlas.DTO;

namespace SismoAtlas.MVC.Services
{
    public class SismoAtlasApiService
    {
        private readonly HttpClient _httpClient;

        // Utilizamos el HttpClient registrado en Program.cs.
        public SismoAtlasApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SismoAtlasApi");
        }

        // Consulta los terremotos y la información de un país.
        public async Task<CountryEarthquakesDto?> BuscarPorPaisAsync(
            string pais,
            CancellationToken cancellationToken = default)
        {
            // Construir la dirección de búsqueda.
            string ruta = $"api/earthquakes/{Uri.EscapeDataString(pais)}";

            // Enviar la solicitud a SismoAtlas.API.
            using var respuesta = await _httpClient.GetAsync(
                ruta, cancellationToken);

            // Si la API devuelve un error, notificamos al controlador.
            respuesta.EnsureSuccessStatusCode();

            // Convertir la respuesta JSON al DTO existente.
            return await respuesta.Content.ReadFromJsonAsync<CountryEarthquakesDto>(
                cancellationToken: cancellationToken);
        }
    }
}
