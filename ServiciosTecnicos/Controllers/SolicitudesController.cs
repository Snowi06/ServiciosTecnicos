using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Filters;


namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "client")]

    public class SolicitudesController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "ServiceRequests");
        }

        public IActionResult Crear()
        {
            return RedirectToAction("Create", "ServiceRequests");
        }

        public IActionResult Detalles(int id)
        {
            return RedirectToAction("Details", "ServiceRequests", new { id });
        }

        public IActionResult Editar(int id)
        {
            return RedirectToAction("Edit", "ServiceRequests", new { id });
        }

        public IActionResult Eliminar(int id)
        {
            return RedirectToAction("Delete", "ServiceRequests", new { id });
        }

        public IActionResult AtenderSiguiente()
        {
            return RedirectToAction("AttendNext", "ServiceRequests");
        }
    }
}
