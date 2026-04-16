using Microsoft.AspNetCore.Mvc;

namespace ServiciosTecnicos.Controllers
{
    public class TecnicosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
