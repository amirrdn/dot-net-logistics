using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logistics.Models;
using Logistics.Data;
using Logistics.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logistics.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, IAuthService authService, ILogger<UserController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            _logger.LogInformation($"Username saat ini: {username}");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = null;
            return View(user);
        }

        public async Task<IActionResult> Profile()
        {
            _logger.LogDebug("=== PROFILE ACCESSED ===");
            _logger.LogDebug($"Full URL: {Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var username = User.Identity?.Name;
            _logger.LogDebug($"User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogDebug($"User.Identity.AuthenticationType: {User.Identity?.AuthenticationType}");
            _logger.LogDebug($"User.Identity.Name: {User.Identity?.Name}");
            _logger.LogDebug($"User.Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
            _logger.LogDebug($"URL Profile: {Request.Path}");
            
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("User tidak terautentikasi, redirecting ke login");
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                _logger.LogWarning($"User dengan username {username} tidak ditemukan");
                return NotFound();
            }

            _logger.LogInformation($"Profile user {username} berhasil diakses");
            user.Password = null;
            return View("~/Views/User/Index.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = _authService.HashPassword(model.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile berhasil diperbarui";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Terjadi kesalahan saat memperbarui profile");
                return View("Index", model);
            }
        }
    }
} 