using Microsoft.AspNetCore.Mvc;
using SismoAtlas.API.Services;
using SismoAtlas.API.Validation;
using SismoAtlas.DTO;

namespace SismoAtlas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EarthquakesController : ControllerBase
    {
        private readonly IEarthquakeSearchService _searchService;

        public EarthquakesController(IEarthquakeSearchService searchService)
        {
            _searchService = searchService;
        }

        // GET api/earthquakes/Costa Rica
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(CountryEarthquakesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CountryEarthquakesDto>> GetByCountry(string name, CancellationToken cancellationToken)
        {
            if (!CountryNameValidator.IsValid(name))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Nombre de país inválido",
                    Detail = "Escriba únicamente el nombre de un país (solo letras, máximo 100 caracteres)."
                });
            }

            var result = await _searchService.SearchByCountryAsync(name, cancellationToken);

            if (result is null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "País no encontrado",
                    Detail = $"No se encontró información ni sismos para \"{name.Trim()}\"."
                });
            }

            return Ok(result);
        }
    }
}