namespace ApiServer.Application.DTOs.Utente
{
    // DTO per aggiornare un utente
    public class UtenteUpdateDTO
    {
        // Dati anagrafici modificabili
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;

        // Dati di contatto / spedizione modificabili
        public string? Indirizzo { get; set; }
        public string? Citta { get; set; }
        public string? CAP { get; set; }
        public string? Telefono { get; set; }

        // Password (opzionale, se l'utente vuole cambiarla)
        public string? NuovaPassword { get; set; }
    }
}

