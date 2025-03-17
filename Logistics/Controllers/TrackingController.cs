using Microsoft.AspNetCore.Mvc;
using Logistics.Models;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Controllers
{
    public class TrackingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrackingController> _logger;

        public TrackingController(ApplicationDbContext context, ILogger<TrackingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var trackingHistory = await _context.ShipmentStatuses
                .Include(s => s.Shipment)
                .OrderByDescending(s => s.StatusDate)
                .ToListAsync();

            return View(trackingHistory);
        }
    }
} 