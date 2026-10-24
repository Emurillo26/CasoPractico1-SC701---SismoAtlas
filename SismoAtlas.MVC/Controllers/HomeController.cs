using Microsoft.AspNetCore.Mvc;

namespace SismoAtlas.MVC.Controllers
{
    // Controller de la única pantalla de la app.
    // TODO (MVC): agregar la acción de búsqueda por país con try/catch y validaciones.
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
