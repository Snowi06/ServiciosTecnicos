using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Models; 

namespace ServiciosTecnicos.Controllers
{
    public class SolicitudesController : Controller
    {
        private static Queue<Solicitud> ColaSolicitudes = new Queue<Solicitud>();

        public IActionResult Index()
        {
            return View(ColaSolicitudes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Solicitud solicitud)
        {
            solicitud.Fecha = DateTime.Now;
            solicitud.Estado = "Pendiente";

            ColaSolicitudes.Enqueue(solicitud);

            return RedirectToAction("Index");
        }

        public IActionResult Atender()
        {
            if (ColaSolicitudes.Count > 0)
            {
                ColaSolicitudes.Dequeue();
            }
            return RedirectToAction("Index");
        }

    }
}
