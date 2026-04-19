using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Models;

namespace ServiciosTecnicos.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            // Obtener lista de clientes
                var clients = _context.Clients
                .Include(c => c.User)
                .Select(c => new
                {
                    c.ClientId,
                    FullName = c.User.FirstName + " " + c.User.LastName
                })
                .ToList();

            ViewBag.Clients = new SelectList(clients, "ClientId", "FullName");
            ViewBag.Categories = new SelectList(_context.ServiceCategories, "CategoryId", "CategoryName");

            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            // Remover validaciones innecesarias que puedan estar bloqueando
            ModelState.Remove("Client");
            ModelState.Remove("Category");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("RequestStatus");
            ModelState.Remove("RequestAssignment");
            ModelState.Remove("Service");
            
            if (ModelState.IsValid)
            {
                serviceRequest.CreatedAt = DateTime.Now;
                serviceRequest.RequestStatus = "pending";

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Solicitud creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Recargar listas en caso de error
            var clients = _context.Clients
                .Include(c => c.User)
                .Where(c => c.User != null)
                .Select(c => new
                {
                    c.ClientId,
                    FullName = (c.User.FirstName ?? "") + " " + (c.User.LastName ?? "")
                })
                .ToList();

            ViewBag.Clients = new SelectList(clients, "ClientId", "FullName", serviceRequest.ClientId);
            ViewBag.Categories = new SelectList(_context.ServiceCategories, "CategoryId", "CategoryName", serviceRequest.CategoryId);

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .Include(s => s.RequestAssignment)
                    .ThenInclude(a => a.Technician)
                        .ThenInclude(t => t.User)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            // Cargar técnicos disponibles si la solicitud está pendiente
            if (serviceRequest.RequestStatus == "pending" && serviceRequest.RequestAssignment == null)
            {
                var availableTechnicians = await _context.Technicians
                    .Include(t => t.User)
                    .Include(t => t.TechnicianSpecialties)
                        .ThenInclude(ts => ts.Specialty)
                    .Where(t => t.Available && t.User.IsActive)
                    .Select(t => new
                    {
                        t.TechnicianId,
                        FullName = t.User.FirstName + " " + t.User.LastName + " - " + t.ExperienceYears + " años exp."
                    })
                    .ToListAsync();

                ViewBag.AvailableTechnicians = new SelectList(availableTechnicians, "TechnicianId", "FullName");
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/AssignTechnician
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTechnician(int requestId, int technicianId)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(requestId);
            
            if (serviceRequest == null)
            {
                return NotFound();
            }

            // Verificar que no esté ya asignado
            var existingAssignment = await _context.RequestAssignments
                .FirstOrDefaultAsync(ra => ra.RequestId == requestId);

            if (existingAssignment != null)
            {
                TempData["Error"] = "Esta solicitud ya tiene un técnico asignado";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }

            // Crear la asignación
            var assignment = new RequestAssignment
            {
                RequestId = requestId,
                TechnicianId = technicianId,
                AssignedAt = DateTime.Now
            };

            _context.RequestAssignments.Add(assignment);

            // Actualizar estado de la solicitud
            serviceRequest.RequestStatus = "assigned";
            _context.Update(serviceRequest);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Técnico asignado exitosamente";
            return RedirectToAction(nameof(Details), new { id = requestId });
        }

        // POST: ServiceRequests/StartService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartService(int requestId)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.RequestAssignment)
                .FirstOrDefaultAsync(s => s.RequestId == requestId);

            if (serviceRequest == null || serviceRequest.RequestAssignment == null)
            {
                return NotFound();
            }

            // Verificar que no exista ya un servicio
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.RequestId == requestId);

            if (existingService != null)
            {
                TempData["Error"] = "Ya existe un servicio para esta solicitud";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }

            // Crear el servicio
            var service = new Service
            {
                RequestId = requestId,
                TechnicianId = serviceRequest.RequestAssignment.TechnicianId,
                StartDate = DateTime.Now,
                FinalStatus = "in_progress"
            };

            _context.Services.Add(service);

            // Actualizar estado de la solicitud
            serviceRequest.RequestStatus = "in_progress";
            _context.Update(serviceRequest);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Servicio iniciado exitosamente";
            return RedirectToAction(nameof(Details), new { id = requestId });
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.RequestId == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Solicitud eliminada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/AttendNext
        public async Task<IActionResult> AttendNext()
        {
            var nextRequest = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .Where(s => s.RequestStatus == "pending")
                .OrderBy(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (nextRequest == null)
            {
                TempData["Info"] = "No hay solicitudes pendientes";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Details), new { id = nextRequest.RequestId });
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.RequestId == id);
        }
    }
}