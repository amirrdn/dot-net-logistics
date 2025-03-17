using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class ShipmentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ShipmentId { get; set; }

        [ForeignKey("ShipmentId")]
        public virtual Shipment? Shipment { get; set; }

        [Required(ErrorMessage = "AWB is required")]
        public string? AWB { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        public DateTime StatusDate { get; set; } = DateTime.Now;
    }
}
