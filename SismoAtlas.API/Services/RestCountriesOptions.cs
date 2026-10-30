namespace SismoAtlas.API.Services
{
    public class RestCountriesOptions
    {
        public const string SectionName = "ExternalApis:RestCountries";

        public string BaseUrl { get; set; } = "https://api.restcountries.com/countries/v5/";
        public string ApiKey { get; set; } = string.Empty;

        public int CacheMinutes { get; set; } = 60;

        public int NotFoundCacheMinutes { get; set; } = 5;

        public int TimeoutSeconds { get; set; } = 10;
    }
}