using System.Text.Json.Serialization;

namespace SismoAtlas.DTO.RestCountries
{
  
    public class RcResponse
    {
        [JsonPropertyName("data")]
        public RcData? Data { get; set; }
    }

    public class RcData
    {
        [JsonPropertyName("objects")]
        public List<RcCountry>? Objects { get; set; }

        [JsonPropertyName("meta")]
        public RcMeta? Meta { get; set; }
    }

    public class RcMeta
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class RcCountry
    {
        [JsonPropertyName("names")]
        public RcNames? Names { get; set; }

        [JsonPropertyName("codes")]
        public RcCodes? Codes { get; set; }

        [JsonPropertyName("capitals")]
        public List<RcCapital>? Capitals { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("subregion")]
        public string? Subregion { get; set; }

        // double? por si la API manda decimales; se convierte a long en el mapeo
        [JsonPropertyName("population")]
        public double? Population { get; set; }

        [JsonPropertyName("area")]
        public RcArea? Area { get; set; }

        [JsonPropertyName("languages")]
        public List<RcLanguage>? Languages { get; set; }

        [JsonPropertyName("currencies")]
        public List<RcCurrency>? Currencies { get; set; }

        [JsonPropertyName("timezones")]
        public List<string>? Timezones { get; set; }

        [JsonPropertyName("flag")]
        public RcFlag? Flag { get; set; }
    }

    public class RcNames
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }

        [JsonPropertyName("official")]
        public string? Official { get; set; }

        // Traducciones por código ISO 639-3 (ej. "spa"). Se usa para buscar "Alemania", "Estados Unidos", etc.
        [JsonPropertyName("translations")]
        public Dictionary<string, RcNameForms?>? Translations { get; set; }
    }

    public class RcNameForms
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }

        [JsonPropertyName("official")]
        public string? Official { get; set; }
    }

    public class RcCodes
    {
        [JsonPropertyName("alpha_2")]
        public string? Alpha2 { get; set; }

        [JsonPropertyName("alpha_3")]
        public string? Alpha3 { get; set; }
    }

    public class RcCapital
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class RcArea
    {
        [JsonPropertyName("kilometers")]
        public double? Kilometers { get; set; }
    }

    public class RcLanguage
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class RcCurrency
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }

    public class RcFlag
    {
        [JsonPropertyName("emoji")]
        public string? Emoji { get; set; }

        [JsonPropertyName("url_png")]
        public string? UrlPng { get; set; }

        [JsonPropertyName("url_svg")]
        public string? UrlSvg { get; set; }
    }
}