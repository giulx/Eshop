using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Applicazione.DTOs.Utente
{
    /// <summary>
    /// DTO per la creazione/registrazione di un nuovo utente.
    /// Usato dal client per registrarsi.
    /// </summary>
    public class UtenteCreateDTO
    {
        /// <summary>
        /// Nome dell'utente.
        /// </summary>
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Cognome dell'utente.
        /// </summary>
        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il cognome non può superare i 100 caratteri.")]
        public string Cognome { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo email dell'utente.
        /// </summary>
        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Formato email non valido.")]
        [StringLength(200, ErrorMessage = "L'email non può superare i 200 caratteri.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password in chiaro (verrà hashata dal livello di infrastruttura).
        /// </summary>
        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La password deve contenere almeno 6 caratteri.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo (opzionale).
        /// </summary>
        [StringLength(200, ErrorMessage = "L'indirizzo non può superare i 200 caratteri.")]
        public string? Indirizzo { get; set; }

        /// <summary>
        /// Città (opzionale).
        /// </summary>
        [StringLength(100, ErrorMessage = "La città non può superare i 100 caratteri.")]
        public string? Citta { get; set; }

        /// <summary>
        /// CAP (opzionale).
        /// </summary>
        [StringLength(10, ErrorMessage = "Il CAP non può superare i 10 caratteri.")]
        public string? CAP { get; set; }

        /// <summary>
        /// Telefono (opzionale).
        /// </summary>
        [Phone(ErrorMessage = "Formato telefono non valido.")]
        [StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri.")]
        public string? Telefono { get; set; }

        // 👇 IMPORTANTE:
        // niente IsAdmin qui perché l'utente non deve poterselo dare da solo.
        // Se ti serve creare admin da codice, lo imposti nel service/repository.
    }
}
