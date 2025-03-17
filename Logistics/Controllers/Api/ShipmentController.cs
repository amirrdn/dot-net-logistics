using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Models;
using Logistics.Data;
using System.Data.SqlClient;

namespace Logistics.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShipmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetShipments()
        {
            return await _context.Shipments
                .Include(s => s.ShipmentStatus)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shipment>> GetShipment(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.ShipmentStatus)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
            {
                return NotFound();
            }

            return shipment;
        }
        [HttpGet("AWB/{awb}")]
        public async Task<ActionResult<Shipment>> GetShipmentByAWB(string awb)
        {
            var shipment = await _context.Shipments
                .Include(s => s.ShipmentStatus)
                .FirstOrDefaultAsync(s => s.AWB == awb);

            if (shipment == null)
            {
                return NotFound();
            }

            return shipment;
        }

        [HttpPost]
        public async Task<ActionResult<Shipment>> CreateShipment(Shipment shipment)
        {
            try
            {
                var existingShipment = await _context.Shipments
                    .FirstOrDefaultAsync(s => s.AWB == shipment.AWB);

                if (existingShipment != null)
                {
                    return BadRequest(new { errors = new { AWB = new[] { "AWB sudah terdaftar" } } });
                }

                _context.Shipments.Add(shipment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message?.Contains("duplicate key") == true)
                {
                    return BadRequest(new { errors = new { AWB = new[] { "AWB sudah terdaftar" } } });
                }
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return BadRequest();
            }

            _context.Entry(shipment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipmentExists(id))
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
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShipmentExists(int id)
        {
            return _context.Shipments.Any(e => e.Id == id);
        }
    }
} 