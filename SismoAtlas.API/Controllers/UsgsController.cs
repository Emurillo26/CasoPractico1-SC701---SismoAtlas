using Microsoft.AspNetCore.Mvc;
using SismoAtlas.API.Services;

namespace SismoAtlas.API.Controllers
{
    // Pedimos sismos nuevos a USGS
    [ApiController]
    [Route("api/[controller]")]
    public class UsgsController : ControllerBase
    {
        private readonly IUsgsService _usgsService;

        public UsgsController(IUsgsService usgsService)
        {
            _usgsService = usgsService;
        }

        //guarda solo los sismos que todavía no estan en la BD
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            var nuevos = await _usgsService.CargarNuevosAsync();
            return Ok(new { nuevos });
        }
    }
}
