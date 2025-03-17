namespace Logistics.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;

    public class Shipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "AWB is required.")]
        public string? AWB { get; set; }

        [Required(ErrorMessage = "Sender Name is required.")]
        [Column("S_Name")]
        public string? SenderName { get; set; }

        [Required(ErrorMessage = "Sender Phone is required.")]
        [Column("S_Tlp")]
        public string? SenderPhone { get; set; }

        [Required(ErrorMessage = "Sender Address is required.")]
        [Column("S_Address")]
        public string? SenderAddress { get; set; }

        [Required(ErrorMessage = "Sender Postcode is required.")]
        [Column("S_PostCode")]
        public string? SenderPostcode { get; set; }

        [Required(ErrorMessage = "Recipient Name is required.")]
        [Column("R_Name")]
        public string? RecipientName { get; set; }

        [Required(ErrorMessage = "Recipient Phone is required.")]
        [Column("R_Tlp")]
        public string? RecipientPhone { get; set; }

        [Required(ErrorMessage = "Recipient Address is required.")]
        [Column("R_Address")]
        public string? RecipientAddress { get; set; }

        [Required(ErrorMessage = "Recipient Postcode is required.")]
        [Column("R_PostCode")]
        public string? RecipientPostcode { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Dimension is required.")]
        public string? Dimension { get; set; }

        public string? Status { get; set; } = "Pending";

        public virtual ICollection<ShipmentStatus> ShipmentStatus { get; set; } = new List<ShipmentStatus>();
    }
}
