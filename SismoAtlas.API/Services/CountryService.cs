using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SismoAtlas.DTO;
using SismoAtlas.DTO.RestCountries;

namespace SismoAtlas.API.Services
{
    public class CountryService : ICountryService
    {
        private const string ApiKeyPlaceholder = "AGREGAR_API_KEY_AQUI";
        private const string OmitFields = "response_fields_omit=names.native,leaders";
        private const string OmitLeadersOnly = "response_fields_omit=leaders";

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly RestCountriesOptions _options;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            HttpClient http,
            IMemoryCache cache,
            IOptions<RestCountriesOptions> options,
            ILogger<CountryService> logger)
        {
            _http = http;
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var cleanName = string.Join(' ', name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            var cacheKey = $"restcountries:country:{Normalize(cleanName)}";

            if (_cache.TryGetValue(cacheKey, out CountryDto? cached))
            {
                _logger.LogInformation("REST Countries: CACHE HIT para '{Country}'", cleanName);
                return cached;
            }

            _logger.LogInformation("REST Countries: CACHE MISS para '{Country}', consultando el API", cleanName);
            var country = await SearchAsync(cleanName, cancellationToken);

            var minutes = country is null ? _options.NotFoundCacheMinutes : _options.CacheMinutes;
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };
            _cache.Set(cacheKey, country, cacheOptions);

            return country;
        }

        private async Task<CountryDto?> SearchAsync(string name, CancellationToken ct)
        {
            var escaped = Uri.EscapeDataString(name);

            var objects = await GetObjectsAsync($"names.common/{escaped}?limit=1&{OmitFields}", ct);
            if (objects.Count > 0)
                return Map(objects[0]);

            objects = await GetObjectsAsync($"name?q={escaped}&limit=10&{OmitFields}", ct);
            var found = PickByName(objects, name);
            if (found != null)
                return Map(found);

            objects = await GetObjectsAsync($"names.translations?q={escaped}&limit=10&{OmitLeadersOnly}", ct);
            found = PickByTranslation(objects, name);
            if (found != null)
                return Map(found);

            return null;
        }

        private async Task<List<RcCountry>> GetObjectsAsync(string relativeUrl, CancellationToken ct)
        {
            EnsureApiKey();

            using var response = await _http.GetAsync(relativeUrl, ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<RcCountry>();

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("REST Countries respondió {Status}: {Body}", (int)response.StatusCode, body);
                throw BuildException(response.StatusCode);
            }

            var payload = await response.Content.ReadFromJsonAsync<RcResponse>(JsonOptions, ct);
            return payload?.Data?.Objects ?? new List<RcCountry>();
        }

        private void EnsureApiKey()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey) || _options.ApiKey == ApiKeyPlaceholder)
            {
                throw new CountryApiException(
                    "El servicio de información de países no está configurado (falta la API key).",
                    HttpStatusCode.ServiceUnavailable);
            }
        }

        private static CountryApiException BuildException(HttpStatusCode status)
        {
            return status switch
            {
                HttpStatusCode.Unauthorized => new CountryApiException(
                    "La API key de REST Countries no es válida.", HttpStatusCode.BadGateway),
                HttpStatusCode.Forbidden => new CountryApiException(
                    "La cuenta de REST Countries no tiene acceso o superó su límite mensual.", HttpStatusCode.BadGateway),
                HttpStatusCode.TooManyRequests => new CountryApiException(
                    "Se hicieron demasiadas consultas a REST Countries. Intente de nuevo en unos segundos.", HttpStatusCode.TooManyRequests),
                HttpStatusCode.BadRequest => new CountryApiException(
                    "REST Countries no pudo procesar la búsqueda.", HttpStatusCode.BadRequest),
                _ => new CountryApiException(
                    "El servicio de información de países no está disponible en este momento.", HttpStatusCode.BadGateway)
            };
        }

        private static RcCountry? PickByName(List<RcCountry> list, string query)
        {
            if (list.Count == 0)
                return null;

            var q = Normalize(query);

            var exact = list.FirstOrDefault(c => Same(c.Names?.Common, q) || Same(c.Names?.Official, q));
            if (exact != null)
                return exact;

            var starts = list
                .Where(c => c.Names?.Common != null && Normalize(c.Names.Common).StartsWith(q))
                .OrderBy(c => c.Names!.Common!.Length)
                .FirstOrDefault();
            if (starts != null)
                return starts;

            return list.Count == 1 ? list[0] : null;
        }

        private static RcCountry? PickByTranslation(List<RcCountry> list, string query)
        {
            if (list.Count == 0)
                return null;

            var q = Normalize(query);

            var match = list.FirstOrDefault(c =>
                c.Names?.Translations != null &&
                c.Names.Translations.Values.Any(t => t != null && (Same(t.Common, q) || Same(t.Official, q))));

            return match ?? (list.Count == 1 ? list[0] : null);
        }

        private static bool Same(string? value, string normalizedQuery)
        {
            return value != null && Normalize(value) == normalizedQuery;
        }

        private static string Normalize(string text)
        {
            var decomposed = text.Trim().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);

            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private static CountryDto Map(RcCountry c)
        {
            var cca2 = c.Codes?.Alpha2;

            var capitals = (c.Capitals ?? new List<RcCapital>())
                .Select(x => x.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            var languages = (c.Languages ?? new List<RcLanguage>())
                .Select(l => l.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .Select(n => n!)
                .ToList();

            var currencies = (c.Currencies ?? new List<RcCurrency>())
                .Select(FormatCurrency)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return new CountryDto
            {
                Name = c.Names?.Common ?? c.Names?.Official ?? string.Empty,
                OfficialName = c.Names?.Official,
                Cca2 = cca2,
                Cca3 = c.Codes?.Alpha3,
                Capital = capitals.Count > 0 ? string.Join(", ", capitals) : null,
                Region = c.Region,
                Subregion = c.Subregion,
                Population = c.Population.HasValue ? (long)Math.Round(c.Population.Value) : null,
                AreaKm2 = c.Area?.Kilometers,
                Languages = languages,
                Currencies = currencies,
                Timezones = c.Timezones ?? new List<string>(),
                FlagUrl = c.Flag?.UrlPng
                          ?? (cca2 != null ? $"https://flags.restcountries.com/v5/w320/{cca2.ToLowerInvariant()}.png" : null),
                FlagSvgUrl = c.Flag?.UrlSvg,
                FlagEmoji = c.Flag?.Emoji
            };
        }

        private static string FormatCurrency(RcCurrency cur)
        {
            var label = !string.IsNullOrWhiteSpace(cur.Name) ? cur.Name : cur.Code;
            if (string.IsNullOrWhiteSpace(label))
                return string.Empty;

            return !string.IsNullOrWhiteSpace(cur.Symbol) ? $"{label} ({cur.Symbol})" : label;
        }
    }
}