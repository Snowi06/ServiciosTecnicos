using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Filters;


namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "client")]

    public class CalificacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
