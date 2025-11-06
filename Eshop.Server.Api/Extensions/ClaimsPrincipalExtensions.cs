using System.Security.Claims;

namespace Eshop.Server.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Estensione per ottenere l'ID utente (claim "id") dal token JWT.
        /// </summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("id");
            if (claim is null) return null;
            return int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}

