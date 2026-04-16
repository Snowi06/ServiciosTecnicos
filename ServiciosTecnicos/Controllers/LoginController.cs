using Microsoft.AspNetCore.Mvc;

namespace ServiciosTecnicos.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
