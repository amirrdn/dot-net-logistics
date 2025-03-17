using Logistics.Models;

namespace Logistics.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        string HashPassword(string password);
        Task<User> CreateUserAsync(User user);
    }
} 