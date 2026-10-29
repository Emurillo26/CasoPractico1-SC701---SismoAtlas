using SismoAtlas.DAL.Entities;

namespace SismoAtlas.DAL.Repositories
{
    // Contrato: qué operaciones se pueden hacer con los sismos
    public interface IEarthquakeRepository
    {
        // ¿Ya hay sismos guardados en la BD?
        Task<bool> HasDataAsync();

        // Fecha del sismo más reciente guardado (null si la tabla está vacía)
        Task<DateTime?> GetLatestTimeAsync();

        // Guarda los sismos nuevos, ignorando los que ya existen. Devuelve cuántos se guardaron
        Task<int> AddRangeAsync(IEnumerable<Earthquake> earthquakes);

        // Sismos de un país, del más reciente al más antiguo
        Task<List<Earthquake>> GetByCountryAsync(string country);
    }
}