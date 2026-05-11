using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiciosTecnicos.Data;
using ServiciosTecnicos.Models;

namespace ServiciosTecnicos.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
            HttpContext.Session.SetString("Role", user.Role.RoleName);

            return RedirectByRole(user.Role.RoleName);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        private IActionResult RedirectByRole(string role)
        {
            return role switch
            {
                "admin" => RedirectToAction("Index", "Home"),
                "technician" => RedirectToAction("Index", "Tecnicos"),
                "client" => RedirectToAction("Index", "ServiceRequests"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
