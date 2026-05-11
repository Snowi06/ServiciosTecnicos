using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Filters;

namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "technician")]
    public class HistorialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
