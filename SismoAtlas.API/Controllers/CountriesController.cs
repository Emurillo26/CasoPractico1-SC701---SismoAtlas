using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using SismoAtlas.API.Services;
using SismoAtlas.DTO;

namespace SismoAtlas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private static readonly Regex NameRegex = new(@"^[\p{L}\s.'\-]+$", RegexOptions.Compiled);

        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<CountryDto>> GetByName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100 || !NameRegex.IsMatch(name))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Nombre de país inválido",
                    Detail = "Escriba únicamente el nombre de un país (solo letras, máximo 100 caracteres)."
                });
            }

            var country = await _countryService.GetByNameAsync(name, cancellationToken);

            if (country is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "País no encontrado",
                    Detail = $"No se encontró información para \"{name.Trim()}\"."
                });
            }

            return Ok(country);
        }
    }
}