namespace SismoAtlas.DTO
{
    public class CountryEarthquakesDto
    {
        
        public CountryDto? Country { get; set; }
        public List<EarthquakeDto> Earthquakes { get; set; } = [];
    }
}