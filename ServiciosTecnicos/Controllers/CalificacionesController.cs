using Microsoft.AspNetCore.Mvc;

namespace ServiciosTecnicos.Controllers
{
    public class CalificacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
