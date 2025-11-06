using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.Utente
{
    /// <summary>
    /// DTO per l'aggiornamento dei dati di un utente esistente.
    /// Tutti i campi sono opzionali: vengono aggiornati solo quelli valorizzati.
    /// </summary>
    public class UtenteUpdateDTO
    {
        /// <summary>
        /// Nuovo nome dell'utente.
        /// </summary>
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
        public string? Nome { get; set; }

        /// <summary>
        /// Nuovo cognome dell'utente.
        /// </summary>
        [StringLength(100, ErrorMessage = "Il cognome non può superare i 100 caratteri.")]
        public string? Cognome { get; set; }

        /// <summary>
        /// Nuovo indirizzo (opzionale).
        /// </summary>
        [StringLength(200, ErrorMessage = "L'indirizzo non può superare i 200 caratteri.")]
        public string? Indirizzo { get; set; }

        /// <summary>
        /// Nuova città (opzionale).
        /// </summary>
        [StringLength(100, ErrorMessage = "La città non può superare i 100 caratteri.")]
        public string? Citta { get; set; }

        /// <summary>
        /// Nuovo CAP (opzionale).
        /// </summary>
        [StringLength(10, ErrorMessage = "Il CAP non può superare i 10 caratteri.")]
        public string? CAP { get; set; }

        /// <summary>
        /// Nuovo telefono (opzionale).
        /// </summary>
        [Phone(ErrorMessage = "Formato telefono non valido.")]
        [StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri.")]
        public string? Telefono { get; set; }

        /// <summary>
        /// Nuova password da impostare (opzionale).
        /// Se null o vuota, la password rimane invariata.
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La password deve contenere almeno 6 caratteri.")]
        public string? NuovaPassword { get; set; }

        // ⚠️ niente IsAdmin qui.
        // Se l'admin deve cambiare il ruolo di un utente, meglio fare un DTO separato
        // o un endpoint separato protetto.
    }
}
