using Microsoft.AspNetCore.Mvc;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Models;
using ServiciosTecnicos.Filters;
using System;
using System.Linq;

namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "client")]
    public class CalificacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalificacionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //LISTAR CALIFICACIONES
        public IActionResult Index()
        {
            var ratings = _context.Ratings.ToList();
            return View(ratings);
        }

        //FORMULARIO
        public IActionResult Create(int serviceId)
        {
            var rating = new Rating
            {
                ServiceId = serviceId
            };

            return View(rating);
        }

        //GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rating rating)
        {
            // Validar servicio
            var serviceExists = _context.Services.Any(s => s.ServiceId == rating.ServiceId);

            if (!serviceExists)
            {
                ModelState.AddModelError("", "El servicio no existe");
            }

            // Validar duplicado
            var exists = _context.Ratings.Any(r => r.ServiceId == rating.ServiceId);

            if (exists)
            {
                ModelState.AddModelError("", "Este servicio ya fue calificado");
            }

            if (!ModelState.IsValid)
            {
                return View(rating);
            }

            try
            {
                rating.CreatedAt = DateTime.Now;

                _context.Ratings.Add(rating);
                _context.SaveChanges();

                TempData["Success"] = "Calificación guardada correctamente";

                return RedirectToAction("Index", "Historial");
            }
            catch
            {
                ModelState.AddModelError("", "Error al guardar en la base de datos");
                return View(rating);
            }
        }
    }
}