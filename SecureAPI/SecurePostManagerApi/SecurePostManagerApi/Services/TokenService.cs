using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SecurePostManagerApi.Models;
using Microsoft.Extensions.Configuration;

namespace SecurePostManagerApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;

            // Récupère la clé secrète de appsettings.json
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"] ??
                throw new InvalidOperationException("JWT Key not configured in appsettings.")));
        }

        public string CreateToken(ApplicationUser user)
        {
            // 1. Définir les "Claims" (les informations que contient le token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id) // ID unique de l'utilisateur
            };

            // 2. Définir les identifiants de signature
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 3. Créer le Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:DurationInMinutes"]!)),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = credentials
            };

            // 4. Créer et écrire le token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}