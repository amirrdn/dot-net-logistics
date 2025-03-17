using Logistics.Models;
using Logistics.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShipmentController> _logger;


        public ShipmentController(ApplicationDbContext context, ILogger<ShipmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var query = _context.Shipments
                .Include(s => s.ShipmentStatus)
                .AsNoTracking();

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var shipments = query
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            foreach (var shipment in shipments)
            {
                _logger.LogInformation($"Shipment ID: {shipment.Id}, AWB: {shipment.AWB}");
                _logger.LogInformation($"ShipmentStatus Count: {shipment.ShipmentStatus?.Count ?? 0}");
                if (shipment.ShipmentStatus != null && shipment.ShipmentStatus.Any())
                {
                    foreach (var status in shipment.ShipmentStatus)
                    {
                        _logger.LogInformation($"Status: {status.Status}, Date: {status.StatusDate}");
                    }
                }
            }

            return View(shipments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Shipment shipment)
        {
            foreach (var key in Request.Form.Keys)
            {
                _logger.LogInformation($"Form Key: {key}, Value: {Request.Form[key]}");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogError($"Validation Error: {error.ErrorMessage}");
                }
                return View(shipment);
            }

            try
            {
                shipment.Status = "Shipment Pick Up";
                _context.Shipments.Add(shipment);
                _context.SaveChanges();
                _logger.LogInformation($"Shipment created with ID: {shipment.Id}");

                var shipmentStatus = new ShipmentStatus
                {
                    ShipmentId = shipment.Id,
                    AWB = shipment.AWB ?? string.Empty,
                    Status = "Shipment Pick Up",
                    StatusDate = DateTime.Now
                };

                _context.ShipmentStatuses.Add(shipmentStatus);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database Error: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return View(shipment);
            }
        }

        public IActionResult UpdateStatus(int id)
        {
            var shipment = _context.Shipments
                .Include(s => s.ShipmentStatus)
                .FirstOrDefault(s => s.Id == id);
                
            if (shipment == null)
            {
                return NotFound();
            }
            return View(shipment);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            
            var shipment = _context.Shipments.Find(id);
            if (shipment == null)
            {
                _logger.LogError($"Shipment with ID {id} not found");
                return NotFound();
            }

            if (string.IsNullOrEmpty(status))
            {
                _logger.LogError("Status is empty or null");
                TempData["ErrorMessage"] = "Status tidak boleh kosong";
                return View(shipment);
            }

            try
            {

                var shipmentStatus = new ShipmentStatus
                {
                    ShipmentId = shipment.Id,
                    AWB = shipment.AWB ?? string.Empty,
                    Status = status,
                    StatusDate = DateTime.Now
                };

                _context.ShipmentStatuses.Add(shipmentStatus);

                _context.SaveChanges();

                shipment.Status = status;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Status pengiriman berhasil diperbarui";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating status: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Gagal memperbarui status pengiriman";
                return View(shipment);
            }
        }

        public async Task<IActionResult> History(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.ShipmentStatus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
            {
                return NotFound();
            }

            var statusHistory = shipment.ShipmentStatus
                .OrderByDescending(s => s.StatusDate)
                .ToList();

            ViewBag.StatusHistory = statusHistory;
            ViewBag.LatestStatusId = statusHistory.FirstOrDefault()?.Id;

            return View(shipment);
        }

        public IActionResult Edit(int id)
        {
            var shipment = _context.Shipments.Find(id);
            if (shipment == null)
            {
                return NotFound();
            }
            return View(shipment);
        }

        [HttpPost]
        public IActionResult Edit(int id, Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipment);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Data pengiriman berhasil diperbarui";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating shipment: {ex.Message}");
                    TempData["ErrorMessage"] = "Gagal memperbarui data pengiriman";
                }
            }
            return View(shipment);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var shipment = _context.Shipments.Find(id);
            if (shipment == null)
            {
                return NotFound();
            }

            try
            {
                _context.Shipments.Remove(shipment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Data pengiriman berhasil dihapus";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting shipment: {ex.Message}");
                TempData["ErrorMessage"] = "Gagal menghapus data pengiriman";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Tracking()
        {
            return RedirectToAction("Index", "Tracking");
        }
    }
}
