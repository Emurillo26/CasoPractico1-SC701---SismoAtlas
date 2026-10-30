using SismoAtlas.DTO;

namespace SismoAtlas.API.Services
{
    public interface ICountryService
    {
        Task<CountryDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}