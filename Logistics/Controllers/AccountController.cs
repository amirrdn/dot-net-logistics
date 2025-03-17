using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logistics.Models;
using Logistics.Services;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AccountController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.Password = null;
            return View("Index", user);
        }
    }
} 