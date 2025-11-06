namespace Eshop.Server.Applicazione.DTOs.Auth
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public int? UtenteId { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public bool IsAdmin { get; set; }
        public string? Token { get; set; }
    }
}

