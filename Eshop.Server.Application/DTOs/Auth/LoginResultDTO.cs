using System;

namespace Eshop.Server.Application.DTOs.Auth
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }

        /// <summary>
        /// Messaggio leggibile per l'utente (es. "Password errata.", "Nessun account trovato con questa email.")
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Codice tecnico opzionale, utile per distinguere i casi lato client.
        /// Esempi: "user_not_found", "wrong_password", "email_not_confirmed".
        /// </summary>
        public string? ErrorCode { get; set; }

        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool IsAdmin { get; set; }
        public string? Token { get; set; }
    }
}
