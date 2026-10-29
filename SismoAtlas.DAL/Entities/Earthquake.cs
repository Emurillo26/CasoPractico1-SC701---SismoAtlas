using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SismoAtlas.DAL.Entities
{
    // Representa un sismo guardado en la tabla dbo.Earthquakes
    [Table("Earthquakes")]
    public class Earthquake
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; } = string.Empty;   // id que trae USGS

        [Column(TypeName = "decimal(4,2)")]
        public decimal? Magnitude { get; set; }

        [MaxLength(250)]
        public string? Place { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }               // sacado de "place"

        public DateTime Time { get; set; }

        public DateTime? Updated { get; set; }

        [MaxLength(500)]
        public string? Url { get; set; }

        public int? Felt { get; set; }

        [MaxLength(20)]
        public string? Alert { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public bool Tsunami { get; set; }

        public int? Significance { get; set; }

        [MaxLength(10)]
        public string? MagType { get; set; }

        [MaxLength(300)]
        public string? Title { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double? Depth { get; set; }
    }
}