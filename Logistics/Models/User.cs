using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logistics.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username harus diisi")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username harus antara 3-50 karakter")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password harus minimal 6 karakter")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [StringLength(100)]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Role harus diisi")]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }
    }
} 