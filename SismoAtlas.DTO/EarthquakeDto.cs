namespace SismoAtlas.DTO
{
   
    public class EarthquakeDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal? Magnitude { get; set; }
        public string? MagType { get; set; }
        public string? Title { get; set; }
        public string? Place { get; set; }
        public string? Country { get; set; }
        public DateTime Time { get; set; }
        public DateTime? Updated { get; set; }
        public string? Url { get; set; }
        public int? Felt { get; set; }
        public string? Alert { get; set; }
        public string? Status { get; set; }
        public bool Tsunami { get; set; }
        public int? Significance { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Depth { get; set; }
    }
}