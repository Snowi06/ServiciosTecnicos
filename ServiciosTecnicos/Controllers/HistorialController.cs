using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Filters;

namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "technician", "client")]
    public class HistorialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            var query = _context.Services
                .Include(s => s.Request)
                    .ThenInclude(r => r.Client)
                        .ThenInclude(c => c.User)
                .Include(s => s.Request)
                    .ThenInclude(r => r.Category)
                .Include(s => s.Technician)
                    .ThenInclude(t => t.User)
                .Where(s => s.FinalStatus == "finalizado" || s.Request.RequestStatus == "finalizado");

            //CLIENTE = solo sus servicios
            if (role?.ToLower() == "client")
            {
                query = query.Where(s => s.Request.Client.UserId == userId);
            }

            //TECNICO = solo los que atendió
            if (role == "technician")
            {
                query = query.Where(s => s.Technician.UserId == userId);
            }

            var finishedServices = await query
                .OrderByDescending(s => s.EndDate ?? s.StartDate)
                .ToArrayAsync();

            return View(finishedServices);
        }
    }
}

