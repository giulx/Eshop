using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Eshop.Server.Infrastructure.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GeneraToken(Utente utente)
        {
            var key = _config["Jwt:Key"] ?? throw new Exception("Jwt:Key mancante");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // Identità
                new Claim("id", utente.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, utente.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, utente.Id.ToString()),

                // Dati utente
                new Claim(JwtRegisteredClaimNames.Email, utente.Email.Valore),
                new Claim("name", $"{utente.Nome} {utente.Cognome}"),

                // Autorizzazione
                new Claim("is_admin", utente.IsAdmin.ToString().ToLowerInvariant())
                // Se in futuro vuoi usare [Authorize(Roles="Admin")], aggiungi:
                // new Claim(ClaimTypes.Role, utente.IsAdmin ? "Admin" : "Cliente")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
