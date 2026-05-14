using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Filters;
using ServiciosTecnicos.Models;

namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "technician")]
    public class TecnicosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TecnicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tecnicos = await _context.Technicians
                .OrderBy(t => t.TechnicianId)
                .ToListAsync();

            return View(tecnicos);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tecnico = await _context.Technicians.FindAsync(id);

            if (tecnico == null)
            {
                return NotFound();
            }

            return View(tecnico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Technician tecnico)
        {
            if (id != tecnico.TechnicianId)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("TechnicianSpecialties");
            ModelState.Remove("RequestAssignments");
            ModelState.Remove("Services");

            if (!ModelState.IsValid)
            {
                return View(tecnico);
            }

            _context.Update(tecnico);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

