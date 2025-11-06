namespace Eshop.Server.Applicazione.DTOs.Utente
{
    /// <summary>
    /// DTO di lettura per i dati dell'utente.
    /// </summary>
    public class UtenteReadDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // dati “non sensibili” che puoi mostrare
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? CAP { get; set; }
        public string? Telefono { get; set; }

        // se vuoi mostrarlo all’admin:
        public bool IsAdmin { get; set; }
    }
}
