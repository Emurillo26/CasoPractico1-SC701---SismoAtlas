using Microsoft.EntityFrameworkCore;
using SismoAtlas.DAL.Context;
using SismoAtlas.DAL.Entities;

namespace SismoAtlas.DAL.Repositories
{
    // Implementación del repositorio usando Entity Framework Core
    public class EarthquakeRepository : IEarthquakeRepository
    {
        private readonly SismoAtlasDbContext _context;

        public EarthquakeRepository(SismoAtlasDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasDataAsync()
        {
            return await _context.Earthquakes.AnyAsync();
        }

        public async Task<DateTime?> GetLatestTimeAsync()
        {
            return await _context.Earthquakes
                .MaxAsync(e => (DateTime?)e.Time);
        }

        public async Task<int> AddRangeAsync(IEnumerable<Earthquake> earthquakes)
        {
            // Quita repetidos dentro de la misma lista
            var nuevos = earthquakes
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .ToList();

            if (nuevos.Count == 0)
                return 0;

            // Busca cuáles de esos Id ya están en la BD
            var ids = nuevos.Select(e => e.Id).ToList();
            var existentes = await _context.Earthquakes
                .Where(e => ids.Contains(e.Id))
                .Select(e => e.Id)
                .ToListAsync();

            // Solo guarda los que no existen
            var aGuardar = nuevos
                .Where(e => !existentes.Contains(e.Id))
                .ToList();

            if (aGuardar.Count == 0)
                return 0;

            await _context.Earthquakes.AddRangeAsync(aGuardar);
            await _context.SaveChangesAsync();

            return aGuardar.Count;
        }

        public async Task<List<Earthquake>> GetByCountryAsync(string country)
        {
            var pais = country.Trim();

            return await _context.Earthquakes
                .AsNoTracking()
                .Where(e => e.Country == pais)
                .OrderByDescending(e => e.Time)
                .ToListAsync();
        }
    }
}