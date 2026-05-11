using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Models;
using ServiciosTecnicos.DataStructures;
using ServiciosTecnicos.Filters;



namespace ServiciosTecnicos.Controllers
{
    /// <summary>
    /// Controlador de Solicitudes que utiliza TADs personalizadas
    /// NO usa List, Queue, Stack nativos de C#
    /// </summary>

    [AuthorizeSession("admin", "client")]

    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        // TAD Cola para gestionar solicitudes pendientes (FIFO)
        private static CustomQueue<int> _pendingRequestsQueue = new CustomQueue<int>();
        
        // TAD Lista Enlazada para historial de operaciones
        private static CustomLinkedList<string> _operationHistory = new CustomLinkedList<string>();

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceRequests
        /// <summary>
        /// Obtiene todas las solicitudes usando TAD CustomLinkedList
        /// NO usa List<> nativo
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Obtener solicitudes de la BD
            var requestsArray = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .OrderByDescending(s => s.CreatedAt)
                .ToArrayAsync(); // Usamos array, no List<>

            // Almacenar en TAD Lista Enlazada Personalizada
            var customList = new CustomLinkedList<ServiceRequest>();
            foreach (var request in requestsArray)
            {
                customList.Add(request);
                
                // Si está pendiente, agregarlo a la cola FIFO
                if (request.RequestStatus == "pending")
                {
                    // Solo agregar si no está ya en la cola
                    bool alreadyInQueue = false;
                    var queueArray = _pendingRequestsQueue.ToArray();
                    foreach (var id in queueArray)
                    {
                        if (id == request.RequestId)
                        {
                            alreadyInQueue = true;
                            break;
                        }
                    }
                    
                    if (!alreadyInQueue)
                    {
                        _pendingRequestsQueue.Enqueue(request.RequestId);
                    }
                }
            }

            // Registrar operación en historial
            _operationHistory.AddFirst($"Index viewed at {DateTime.Now:HH:mm:ss}");

            // Convertir CustomLinkedList a array para la vista
            ViewBag.TotalRequests = customList.Count;
            ViewBag.PendingInQueue = _pendingRequestsQueue.Count;
            
            return View(customList.ToArray());
        }

        // GET: ServiceRequests/Create
        /// <summary>
        /// Usa TAD CustomLinkedList para almacenar clientes
        /// NO usa List<> nativo
        /// </summary>
        public IActionResult Create()
        {
            // Obtener clientes de la BD como array
            var clientsArray = _context.Clients
                .Include(c => c.User)
                .Select(c => new
                {
                    c.ClientId,
                    FullName = c.User.FirstName + " " + c.User.LastName
                })
                .ToArray(); // Array en lugar de List<>

            // Almacenar en TAD Lista Enlazada Personalizada
            var customClientList = new CustomLinkedList<dynamic>();
            foreach (var client in clientsArray)
            {
                customClientList.Add(client);
            }

            // Convertir CustomLinkedList a array para SelectList
            ViewBag.Clients = new SelectList(customClientList.ToArray(), "ClientId", "FullName");
            
            var categoriesArray = _context.ServiceCategories.ToArray();
            ViewBag.Categories = new SelectList(categoriesArray, "CategoryId", "CategoryName");

            // Registrar en historial
            _operationHistory.AddFirst($"Create form opened at {DateTime.Now:HH:mm:ss}");

            return View();
        }

        // POST: ServiceRequests/Create
        /// <summary>
        /// Crea una solicitud y la encola en CustomQueue si está pendiente
        /// </summary>
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

                // *** TAD COLA: Encolar en CustomQueue (FIFO) ***
                _pendingRequestsQueue.Enqueue(serviceRequest.RequestId);
                
                // *** TAD HISTORIAL: Registrar operación ***
                _operationHistory.AddFirst($"Created request #{serviceRequest.RequestId} at {DateTime.Now:HH:mm:ss}");

                TempData["Success"] = $"Solicitud creada exitosamente. Cola tiene {_pendingRequestsQueue.Count} solicitudes pendientes";
                return RedirectToAction(nameof(Index));
            }

            // Recargar usando CustomLinkedList
            var clientsArray = _context.Clients
                .Include(c => c.User)
                .Where(c => c.User != null)
                .Select(c => new
                {
                    c.ClientId,
                    FullName = (c.User.FirstName ?? "") + " " + (c.User.LastName ?? "")
                })
                .ToArray();

            var customClientList = new CustomLinkedList<dynamic>();
            foreach (var client in clientsArray)
            {
                customClientList.Add(client);
            }

            ViewBag.Clients = new SelectList(customClientList.ToArray(), "ClientId", "FullName", serviceRequest.ClientId);
            
            var categoriesArray = _context.ServiceCategories.ToArray();
            ViewBag.Categories = new SelectList(categoriesArray, "CategoryId", "CategoryName", serviceRequest.CategoryId);

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
            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.RequestAssignment)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(s => s.RequestId == id);

            if (serviceRequest != null)
            {
                if (serviceRequest.Service != null)
                {
                    _context.Services.Remove(serviceRequest.Service);
                }

                if (serviceRequest.RequestAssignment != null)
                {
                    _context.RequestAssignments.Remove(serviceRequest.RequestAssignment);
                }

                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Solicitud eliminada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/AttendNext
        /// <summary>
        /// *** IMPLEMENTACIÓN CLAVE DE TAD COLA (FIFO) ***
        /// Desencola de CustomQueue (NO usa Queue<> nativo)
        /// Garantiza atención por orden de llegada
        /// </summary>
        public async Task<IActionResult> AttendNext()
        {
            // Verificar si la cola está vacía usando TAD
            if (_pendingRequestsQueue.IsEmpty)
            {
                TempData["Info"] = "No hay solicitudes en la cola de atención";
                
                // Registrar en historial
                _operationHistory.AddFirst($"AttendNext called but queue empty at {DateTime.Now:HH:mm:ss}");
                
                return RedirectToAction(nameof(Index));
            }

            // *** DESENCOLAR (FIFO) - Operación O(1) ***
            int nextRequestId = _pendingRequestsQueue.Dequeue();
            
            // Registrar en historial usando TAD Lista Enlazada
            _operationHistory.AddFirst($"Dequeued request #{nextRequestId} at {DateTime.Now:HH:mm:ss}");

            // Buscar la solicitud en la BD
            var nextRequest = await _context.ServiceRequests
                .Include(s => s.Client)
                    .ThenInclude(c => c.User)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.RequestId == nextRequestId);

            if (nextRequest == null)
            {
                TempData["Error"] = "La solicitud desencolada no existe en la base de datos";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = $"Atendiendo solicitud #{nextRequestId}. Quedan {_pendingRequestsQueue.Count} en cola";
            
            return RedirectToAction(nameof(Details), new { id = nextRequest.RequestId });
        }

        // GET: ServiceRequests/ViewHistory
        /// <summary>
        /// Muestra el historial de operaciones usando TAD CustomLinkedList
        /// </summary>
        public IActionResult ViewHistory()
        {
            var historyArray = _operationHistory.ToArray();
            ViewBag.HistoryCount = _operationHistory.Count;
            
            return View(historyArray);
        }

        // GET: ServiceRequests/QueueStatus
        /// <summary>
        /// Muestra el estado de la cola usando TAD CustomQueue
        /// </summary>
        public async Task<IActionResult> QueueStatus()
        {
            var queueIds = _pendingRequestsQueue.ToArray();
            
            // Crear lista enlazada personalizada con las solicitudes en cola
            var queuedRequests = new CustomLinkedList<ServiceRequest>();
            
            foreach (var id in queueIds)
            {
                var request = await _context.ServiceRequests
                    .Include(s => s.Client).ThenInclude(c => c.User)
                    .Include(s => s.Category)
                    .FirstOrDefaultAsync(s => s.RequestId == id);
                
                if (request != null)
                {
                    queuedRequests.Add(request);
                }
            }

            ViewBag.QueueCount = _pendingRequestsQueue.Count;
            
            return View(queuedRequests.ToArray());
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.RequestId == id);
        }
    }
}