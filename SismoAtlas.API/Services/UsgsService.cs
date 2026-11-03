using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using SismoAtlas.DAL.Entities;
using SismoAtlas.DAL.Repositories;
using SismoAtlas.DTO.Usgs;

namespace SismoAtlas.API.Services
{

    public class UsgsService : IUsgsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IEarthquakeRepository _repository;


        private static readonly HashSet<string> EstadosUsa = new(StringComparer.OrdinalIgnoreCase)
        {
            "Alabama","AL","Alaska","AK","Arizona","AZ","Arkansas","AR","California","CA","Colorado","CO",
            "Connecticut","CT","Delaware","DE","Florida","FL","Georgia","GA","Hawaii","HI","Idaho","ID",
            "Illinois","IL","Indiana","IN","Iowa","IA","Kansas","KS","Kentucky","KY","Louisiana","LA",
            "Maine","ME","Maryland","MD","Massachusetts","MA","Michigan","MI","Minnesota","MN",
            "Mississippi","MS","Missouri","MO","Montana","MT","Nebraska","NE","Nevada","NV",
            "New Hampshire","NH","New Jersey","NJ","New Mexico","NM","New York","NY","North Carolina","NC",
            "North Dakota","ND","Ohio","OH","Oklahoma","OK","Oregon","OR","Pennsylvania","PA",
            "Rhode Island","RI","South Carolina","SC","South Dakota","SD","Tennessee","TN","Texas","TX",
            "Utah","UT","Vermont","VT","Virginia","VA","Washington","WA","West Virginia","WV",
            "Wisconsin","WI","Wyoming","WY","District of Columbia","DC"
        };


        private static readonly Dictionary<string, string> OtrosNombres = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Burma (Myanmar)"] = "Myanmar",
            ["Czech Republic"] = "Czechia",
            ["Democratic Republic of the Congo"] = "DR Congo",
            ["Macedonia"] = "North Macedonia"
        };


        public UsgsService(
            HttpClient httpClient,
            IMemoryCache cache,
            IConfiguration configuration,
            IEarthquakeRepository repository)
        {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
            _repository = repository;
        }

        public async Task CargaInicialAsync()
        {

            if (await _repository.HasDataAsync())
                return;

            var inicio = LeerFecha("ExternalApis:Usgs:StartTime") ?? new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var fin = LeerFecha("ExternalApis:Usgs:EndTime") ?? DateTime.UtcNow;

            await CargarRangoAsync(inicio, fin);
        }

        public async Task<int> CargarNuevosAsync()
        {

            var ultimo = await _repository.GetLatestTimeAsync();
            var inicio = ultimo.HasValue
                ? DateTime.SpecifyKind(ultimo.Value, DateTimeKind.Utc)
                : LeerFecha("ExternalApis:Usgs:StartTime") ?? new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return await CargarRangoAsync(inicio, DateTime.UtcNow);
        }


        private async Task<int> CargarRangoAsync(DateTime inicio, DateTime fin)
        {
            var total = 0;
            var desde = inicio;

            while (desde < fin)
            {
                var hasta = desde.AddMonths(1);
                if (hasta > fin)
                    hasta = fin;

                var features = await ObtenerSismosAsync(desde, hasta);


                var entidades = new List<Earthquake>();
                foreach (var feature in features)
                {
                    var entidad = ConvertToEntity(feature);
                    if (entidad != null)
                        entidades.Add(entidad);
                }


                total += await _repository.AddRangeAsync(entidades);

                desde = hasta;
            }

            return total;
        }


        private async Task<List<UsgsFeature>> ObtenerSismosAsync(DateTime desde, DateTime hasta)
        {
            var cacheKey = $"usgs_{desde:yyyyMMddHHmmss}_{hasta:yyyyMMddHHmmss}";

            if (_cache.TryGetValue(cacheKey, out List<UsgsFeature>? enCache) && enCache != null)
                return enCache;

            var baseUrl = _configuration["ExternalApis:Usgs:BaseUrl"];
            var minMagnitud = _configuration["ExternalApis:Usgs:MinMagnitude"];

            var url = $"{baseUrl}query?format=geojson"
                      + $"&starttime={desde.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)}"
                      + $"&endtime={hasta.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)}"
                      + $"&minmagnitude={minMagnitud}"
                      + "&orderby=time-asc&limit=20000";


            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "SismoAtlas");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            var sismos = new List<UsgsFeature>();


            if (response.StatusCode != HttpStatusCode.NoContent)
            {

                response.EnsureSuccessStatusCode();


                var contenido = await response.Content.ReadAsStringAsync();
                var datos = JsonSerializer.Deserialize<UsgsResponse>(contenido);
                sismos = datos?.Features ?? new List<UsgsFeature>();
            }

            var minutos = _configuration.GetValue<int>("ExternalApis:Usgs:CacheMinutes", 10);
            _cache.Set(cacheKey, sismos, TimeSpan.FromMinutes(minutos));

            return sismos;
        }


        private static Earthquake? ConvertToEntity(UsgsFeature feature)
        {
            var props = feature.Properties;
            var coords = feature.Geometry?.Coordinates;


            if (string.IsNullOrWhiteSpace(feature.Id) || props == null
                || coords == null || coords.Count < 2 || coords[0] == null || coords[1] == null)
            {
                return null;
            }

            return new Earthquake
            {
                Id = Cortar(feature.Id, 50)!,
                Magnitude = props.Mag,
                Place = Cortar(props.Place, 250),
                Country = Cortar(ObtenerPais(props.Place), 100),

                Time = DateTimeOffset.FromUnixTimeMilliseconds(props.Time).UtcDateTime,
                Updated = props.Updated.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(props.Updated.Value).UtcDateTime : null,
                Url = Cortar(props.Url, 500),
                Felt = props.Felt,
                Alert = Cortar(props.Alert, 20),
                Status = Cortar(props.Status, 20),

                Tsunami = props.Tsunami == 1,
                Significance = props.Sig,
                MagType = Cortar(props.MagType, 10),
                Title = Cortar(props.Title, 300),

                Longitude = coords[0]!.Value,
                Latitude = coords[1]!.Value,
                Depth = coords.Count > 2 ? coords[2] : null
            };
        }


        private static string? ObtenerPais(string? place)
        {
            if (string.IsNullOrWhiteSpace(place))
                return null;

            var coma = place.LastIndexOf(',');
            var pais = (coma >= 0 ? place[(coma + 1)..] : place).Trim();


            if (pais.EndsWith(" region", StringComparison.OrdinalIgnoreCase))
                pais = pais[..^" region".Length].Trim();

            if (pais.Length == 0)
                return null;

            if (EstadosUsa.Contains(pais))
                return "United States";

            return OtrosNombres.TryGetValue(pais, out var otro) ? otro : pais;
        }

        private static string? Cortar(string? texto, int maximo)
        {
            if (texto == null)
                return null;

            return texto.Length <= maximo ? texto : texto[..maximo];
        }

        private DateTime? LeerFecha(string clave)
        {
            var texto = _configuration[clave];
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            return DateTime.TryParse(texto, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var fecha)
                ? fecha
                : null;
        }
    }
}
