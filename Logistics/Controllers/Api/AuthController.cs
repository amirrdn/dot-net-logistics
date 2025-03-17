using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logistics.Models;
using Logistics.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Logistics.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IJwtService jwtService, IAuthService authService, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User model)
        {
            try
            {
                var user = await _authService.CreateUserAsync(model);
                return Ok(new { 
                    message = "User berhasil dibuat",
                    user = new {
                        user.Id,
                        user.Username,
                        user.FullName,
                        user.Email,
                        user.Role,
                        user.CreatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat membuat user" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login(LoginModel loginModel)
        {
            var user = await _authService.ValidateUserAsync(loginModel.Username, loginModel.Password);
            
            if (user == null)
                return Unauthorized(new { message = "Username atau password salah" });

            var token = _jwtService.GenerateToken(user.Username);
            var expiration = DateTime.UtcNow.AddMinutes(60);

            return Ok(new TokenResponse
            {
                Token = token,
                Expiration = expiration
            });
        }

        [HttpGet("validate")]
        [Authorize]
        public ActionResult ValidateToken()
        {
            var username = User.Identity?.Name;
            return Ok(new { 
                message = "Token valid",
                username = username
            });
        }

        [HttpGet("check-token")]
        public ActionResult CheckToken([FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token tidak ditemukan atau format tidak valid" });
            }

            var token = authorization.Replace("Bearer ", "").Trim();
            Console.WriteLine("\n=== Token Debug Info ===");
            Console.WriteLine($"Raw token: {token}");
            Console.WriteLine($"Token length: {token.Length}");
            Console.WriteLine($"Number of parts: {token.Split('.').Length}");

            var parts = token.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                Console.WriteLine($"Part {i + 1}:");
                Console.WriteLine($"- Length: {parts[i].Length}");
                Console.WriteLine($"- Content: {parts[i]}");
            }

            Console.WriteLine("\n=== Validation Parameters ===");
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key tidak ditemukan"));
            Console.WriteLine($"Key length: {key.Length}");
            Console.WriteLine($"Issuer: {_configuration["Jwt:Issuer"]}");
            Console.WriteLine($"Audience: {_configuration["Jwt:Audience"]}");

            if (!_jwtService.ValidateToken(token, out string errorMessage))
            {
                Console.WriteLine("\n=== Error Details ===");
                Console.WriteLine($"Error message: {errorMessage}");
                return Unauthorized(new { message = "Token tidak valid", error = errorMessage });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var username = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value 
                          ?? jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Username tidak ditemukan dalam token" });
            }

            return Ok(new { 
                message = "Token valid",
                username = username
            });
        }

        [HttpGet("debug-token")]
        public ActionResult DebugToken([FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Token tidak ditemukan atau format tidak valid" });
            }

            var token = authorization.Substring("Bearer ".Length).Trim();
            var parts = token.Split('.');
            
            var result = new
            {
                TokenLength = token.Length,
                PartsCount = parts.Length,
                PartsLength = parts.Select((p, i) => new { Part = i + 1, Length = p.Length }).ToList(),
                HasInvalidChars = token.Any(c => !char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_'),
                InvalidChars = token.Where(c => !char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_')
                    .Select(c => new { Char = c, Code = (int)c })
                    .ToList()
            };

            return Ok(result);
        }
    }
} 