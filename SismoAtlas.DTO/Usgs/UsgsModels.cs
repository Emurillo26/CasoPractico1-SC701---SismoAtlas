using System.Text.Json.Serialization;

namespace SismoAtlas.DTO.Usgs
{
    public class UsgsResponse
    {
        [JsonPropertyName("features")]
        public List<UsgsFeature>? Features { get; set; }
    }

    // Cada elemento de "features" es un sismo
    public class UsgsFeature
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("properties")]
        public UsgsProperties? Properties { get; set; }

        [JsonPropertyName("geometry")]
        public UsgsGeometry? Geometry { get; set; }
    }

    public class UsgsProperties
    {
        [JsonPropertyName("mag")]
        public decimal? Mag { get; set; }

        [JsonPropertyName("place")]
        public string? Place { get; set; }

        // Milisegundos desde 1970 (Unix epoch). Se convierte a DateTime en el mapeo.
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("updated")]
        public long? Updated { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("felt")]
        public int? Felt { get; set; }

        [JsonPropertyName("alert")]
        public string? Alert { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        // USGS manda 0 o 1, no true/false. Se convierte a bool en el mapeo.
        [JsonPropertyName("tsunami")]
        public int Tsunami { get; set; }

        [JsonPropertyName("sig")]
        public int? Sig { get; set; }

        [JsonPropertyName("magType")]
        public string? MagType { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }

    public class UsgsGeometry
    {
        // Orden que usa GeoJSON: [longitud, latitud, profundidad]
        [JsonPropertyName("coordinates")]
        public List<double?>? Coordinates { get; set; }
    }
}