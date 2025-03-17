using Microsoft.AspNetCore.Mvc;
using Logistics.Data;
using Logistics.Models;
using System.Linq;

namespace Logistics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShipmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetShipments()
        {
            return Ok(_context.Shipments.ToList());
        }

        [HttpGet("{awb}")]
        public IActionResult GetShipment(string awb)
        {
            var shipment = _context.Shipments.FirstOrDefault(s => s.AWB == awb);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }

        [HttpPost]
        public IActionResult CreateShipment([FromBody] Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetShipment), new { awb = shipment.AWB }, shipment);
        }

        [HttpPut("{awb}")]
        public IActionResult UpdateShipmentStatus(string awb, [FromBody] ShipmentStatus status)
        {
            var shipment = _context.Shipments.FirstOrDefault(s => s.AWB == awb);
            if (shipment == null) return NotFound();

            _context.ShipmentStatuses.Add(status);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
