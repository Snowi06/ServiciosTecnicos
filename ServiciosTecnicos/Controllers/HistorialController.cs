using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Filters;

namespace ServiciosTecnicos.Controllers
{
    [AuthorizeSession("admin", "technician")]
    public class HistorialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var finishedServices = await _context.Services
                .Include(s => s.Request)
                    .ThenInclude(r => r.Client)
                        .ThenInclude(c => c.User)
                .Include(s => s.Request)
                    .ThenInclude(r => r.Category)
                .Include(s => s.Technician)
                    .ThenInclude(t => t.User)
                .Where(s => s.FinalStatus == "finalizado" || s.Request.RequestStatus == "finalizado")
                .OrderByDescending(s => s.EndDate ?? s.StartDate)
                .ToArrayAsync();

            return View(finishedServices);
        }
    }
}

