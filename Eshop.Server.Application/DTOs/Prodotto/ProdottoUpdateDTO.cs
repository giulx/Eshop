using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Applicazione.DTOs.Prodotto
{
    /// <summary>
    /// DTO per l'aggiornamento (parziale o completo) dei dati di un prodotto esistente.
    /// Tutte le proprietà sono opzionali: possono essere aggiornate singolarmente.
    /// </summary>
    public class ProdottoUpdateDTO
    {
        /// <summary>
        /// Nuovo nome del prodotto (opzionale).
        /// </summary>
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
        public string? Nome { get; set; }

        /// <summary>
        /// Nuova descrizione del prodotto (opzionale).
        /// </summary>
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri.")]
        public string? Descrizione { get; set; }

        /// <summary>
        /// Nuovo prezzo del prodotto (opzionale, deve essere maggiore di zero se specificato).
        /// </summary>
        [Range(0.01, 999999.99, ErrorMessage = "Il prezzo deve essere compreso tra 0.01 e 999999.99.")]
        public decimal? Prezzo { get; set; }

        /// <summary>
        /// Nuovo codice valuta (es. "EUR", "USD").
        /// </summary>
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La valuta deve avere 3 lettere (es. EUR).")]
        public string? Valuta { get; set; }

        /// <summary>
        /// Nuova quantità disponibile (opzionale, non può essere negativa).
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore o uguale a zero.")]
        public int? QuantitaDisponibile { get; set; }
    }
}
