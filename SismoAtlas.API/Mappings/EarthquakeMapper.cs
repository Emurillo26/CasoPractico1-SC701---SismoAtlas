using SismoAtlas.DAL.Entities;
using SismoAtlas.DTO;

namespace SismoAtlas.API.Mappings
{
    public static class EarthquakeMapper
    {
        public static EarthquakeDto ToDto(this Earthquake e)
        {
            return new EarthquakeDto
            {
                Id = e.Id,
                Magnitude = e.Magnitude,
                MagType = e.MagType,
                Title = e.Title,
                Place = e.Place,
                Country = e.Country,
                Time = AsUtc(e.Time),
                Updated = e.Updated.HasValue ? AsUtc(e.Updated.Value) : null,
                Url = e.Url,
                Felt = e.Felt,
                Alert = e.Alert,
                Status = e.Status,
                Tsunami = e.Tsunami,
                Significance = e.Significance,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                Depth = e.Depth
            };
        }

        public static List<EarthquakeDto> ToDtoList(this IEnumerable<Earthquake> earthquakes)
        {
            return earthquakes.Select(e => e.ToDto()).ToList();
        }

        private static DateTime AsUtc(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}