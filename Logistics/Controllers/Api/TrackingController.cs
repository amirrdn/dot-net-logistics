using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Models;
using Logistics.Data;

namespace Logistics.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentStatus>>> GetTrackingHistory()
        {
            return await _context.ShipmentStatuses
                .Include(s => s.Shipment)
                .OrderByDescending(s => s.StatusDate)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentStatus>> GetTrackingStatus(int id)
        {
            var trackingStatus = await _context.ShipmentStatuses
                .Include(s => s.Shipment)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (trackingStatus == null)
            {
                return NotFound();
            }

            return trackingStatus;
        }

        [HttpGet("AWB/{awb}")]
        public async Task<ActionResult<IEnumerable<ShipmentStatus>>> GetTrackingHistoryByAWB(string awb)
        {
            var trackingHistory = await _context.ShipmentStatuses
                .Include(s => s.Shipment)
                .Where(s => s.AWB == awb)
                .OrderByDescending(s => s.StatusDate)
                .ToListAsync();

            if (!trackingHistory.Any())
            {
                return NotFound();
            }

            return trackingHistory;
        }

        [HttpPost]
        public async Task<ActionResult<ShipmentStatus>> CreateTrackingStatus(ShipmentStatus trackingStatus)
        {
            _context.ShipmentStatuses.Add(trackingStatus);
            await _context.SaveChangesAsync();

            var shipment = await _context.Shipments.FindAsync(trackingStatus.ShipmentId);
            if (shipment != null)
            {
                shipment.Status = trackingStatus.Status;
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetTrackingStatus), new { id = trackingStatus.Id }, trackingStatus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrackingStatus(int id, ShipmentStatus trackingStatus)
        {
            if (id != trackingStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(trackingStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackingStatusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrackingStatus(int id)
        {
            var trackingStatus = await _context.ShipmentStatuses.FindAsync(id);
            if (trackingStatus == null)
            {
                return NotFound();
            }

            _context.ShipmentStatuses.Remove(trackingStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrackingStatusExists(int id)
        {
            return _context.ShipmentStatuses.Any(e => e.Id == id);
        }
    }
} 