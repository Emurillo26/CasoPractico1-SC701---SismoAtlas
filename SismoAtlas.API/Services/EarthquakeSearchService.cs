using SismoAtlas.API.Mappings;
using SismoAtlas.DAL.Repositories;
using SismoAtlas.DTO;

namespace SismoAtlas.API.Services
{
    public class EarthquakeSearchService : IEarthquakeSearchService
    {
        private readonly ICountryService _countryService;
        private readonly IEarthquakeRepository _earthquakeRepository;
        private readonly ILogger<EarthquakeSearchService> _logger;

        public EarthquakeSearchService(
            ICountryService countryService,
            IEarthquakeRepository earthquakeRepository,
            ILogger<EarthquakeSearchService> logger)
        {
            _countryService = countryService;
            _earthquakeRepository = earthquakeRepository;
            _logger = logger;
        }

        public async Task<CountryEarthquakesDto?> SearchByCountryAsync(string name, CancellationToken cancellationToken = default)
        {
            var typedName = name.Trim();
            var country = await _countryService.GetByNameAsync(typedName, cancellationToken);
            var dbName = country?.Name ?? typedName;

            var entities = await _earthquakeRepository.GetByCountryAsync(dbName);

            _logger.LogInformation(
                "Búsqueda '{Typed}': país en REST Countries = {HasCountry}, sismos en BD para '{DbName}' = {Count}",
                typedName, country != null, dbName, entities.Count);

            
            if (country is null && entities.Count == 0)
                return null;

           
            return new CountryEarthquakesDto { Country = country, Earthquakes = entities.ToDtoList() };
        }
    }
}