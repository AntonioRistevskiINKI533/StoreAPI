using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StoreAPI.Services
{
    public class TokenService
    {
        private const string SecretKey = "mysupersecretkey12345mysupersecretkey12345"; // Minimum 16 characters // nope it had to be 32 otherwise it threw an error, TODO add better value
        private readonly SymmetricSecurityKey _key;

        public TokenService()
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }

        public string GenerateToken(string username, string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // stands for "JWT ID", and it's a unique identifier for each token. Helps prevent token replay attacks
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "StoreApp",
                audience: "StoreAppUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(72),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "StoreApp",
                ValidateAudience = true,
                ValidAudience = "StoreAppUsers",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}
