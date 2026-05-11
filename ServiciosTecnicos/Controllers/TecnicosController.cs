using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Filters;


namespace ServiciosTecnicos.Controllers
{
    public class TecnicosController : Controller
    {
        [AuthorizeSession("admin", "technician")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
