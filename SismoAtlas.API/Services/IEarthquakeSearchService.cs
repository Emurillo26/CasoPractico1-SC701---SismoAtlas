using SismoAtlas.DTO;

namespace SismoAtlas.API.Services
{
    public interface IEarthquakeSearchService
    {
        // Busca los sismos de un país y trae la información del país en un solo objeto.
        Task<CountryEarthquakesDto?> SearchByCountryAsync(string name, CancellationToken cancellationToken = default);
    }
}