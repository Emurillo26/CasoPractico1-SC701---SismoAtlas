using System.Text.Json.Serialization;

namespace SismoAtlas.DTO
{
    
    public class ApiErrorDto
    {
        
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}