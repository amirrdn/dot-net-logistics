using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Logistics.Models;
using System.Text.Json;

namespace Logistics.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username);
        bool ValidateToken(string token, out string errorMessage);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationInMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found");
            _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found");
            _expirationInMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60");
        }

        public bool ValidateToken(string token, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    errorMessage = "Token tidak memiliki format yang valid (harus memiliki 3 bagian)";
                    return false;
                }

                var header = parts[0];
                var headerBytes = Base64UrlEncoder.DecodeBytes(header);
                var headerJson = Encoding.UTF8.GetString(headerBytes);
                Console.WriteLine($"Header JSON: {headerJson}");

                var payload = parts[1];
                var payloadBytes = Base64UrlEncoder.DecodeBytes(payload);
                var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                Console.WriteLine($"Payload JSON: {payloadJson}");

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    AudienceValidator = (audiences, token, parameters) =>
                    {
                        if (audiences == null || !audiences.Any())
                            return false;
                        return audiences.Contains(_audience);
                    }
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error validasi token: {ex.Message}";
                return false;
            }
        }

        public string GenerateToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, EpochTime.GetIntDate(now.AddMinutes(_expirationInMinutes)).ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, EpochTime.GetIntDate(now).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _audience)
            };

            Console.WriteLine($"Generating token with:");
            Console.WriteLine($"- Key length: {_key.Length}");
            Console.WriteLine($"- Issuer: {_issuer}");
            Console.WriteLine($"- Audience: {_audience}");
            Console.WriteLine($"- Expiration: {now.AddMinutes(_expirationInMinutes)}");

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_expirationInMinutes),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var generatedToken = tokenHandler.WriteToken(token);

            if (!ValidateToken(generatedToken, out string errorMessage))
            {
                throw new InvalidOperationException($"Token yang dihasilkan tidak valid: {errorMessage}");
            }

            Console.WriteLine($"Generated token parts:");
            for (int i = 0; i < generatedToken.Split('.').Length; i++)
            {
                Console.WriteLine($"Part {i + 1}:");
                Console.WriteLine($"- Length: {generatedToken.Split('.')[i].Length}");
                Console.WriteLine($"- Content: {generatedToken.Split('.')[i]}");
            }

            return generatedToken;
        }
    }
} 