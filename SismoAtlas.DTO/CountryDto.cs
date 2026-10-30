namespace SismoAtlas.DTO
{
   
    public class CountryDto
    {
        public string Name { get; set; } = string.Empty;       
        public string? OfficialName { get; set; }             
        public string? Cca2 { get; set; }                     
        public string? Cca3 { get; set; }                      
        public string? Capital { get; set; }
        public string? Region { get; set; }
        public string? Subregion { get; set; }
        public long? Population { get; set; }
        public double? AreaKm2 { get; set; }
        public List<string> Languages { get; set; } = [];
        public List<string> Currencies { get; set; } = [];  
        public List<string> Timezones { get; set; } = [];
        public string? FlagUrl { get; set; }                   
        public string? FlagSvgUrl { get; set; }               
        public string? FlagEmoji { get; set; }
    }
}