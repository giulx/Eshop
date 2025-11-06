namespace ApiServer.Application.DTOs.Utente
{
    public class UtenteReadDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}

