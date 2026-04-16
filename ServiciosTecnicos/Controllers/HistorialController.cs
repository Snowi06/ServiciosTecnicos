using Microsoft.AspNetCore.Mvc;

namespace ServiciosTecnicos.Controllers
{
    public class HistorialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
