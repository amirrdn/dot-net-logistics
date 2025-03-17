using System.ComponentModel.DataAnnotations;

namespace Logistics.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }

    public class TokenResponse
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
} 