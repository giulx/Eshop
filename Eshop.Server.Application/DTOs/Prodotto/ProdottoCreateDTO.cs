using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.Prodotto
{
    /// <summary>
    /// DTO per la creazione di un nuovo prodotto nel catalogo.
    /// </summary>
    public class ProdottoCreateDTO
    {
        /// <summary>
        /// Nome del prodotto.
        /// </summary>
        [Required(ErrorMessage = "Il nome del prodotto è obbligatorio.")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione testuale del prodotto.
        /// </summary>
        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri.")]
        public string? Descrizione { get; set; }

        /// <summary>
        /// Prezzo unitario del prodotto (deve essere maggiore di zero).
        /// </summary>
        [Required(ErrorMessage = "Il prezzo è obbligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "Il prezzo deve essere compreso tra 0.01 e 999999.99.")]
        public decimal Prezzo { get; set; }

        /// <summary>
        /// Codice della valuta (es. "EUR", "USD"). Di default è "EUR".
        /// </summary>
        [StringLength(3, MinimumLength = 3, ErrorMessage = "La valuta deve avere 3 lettere (es. EUR).")]
        public string Valuta { get; set; } = "EUR";

        /// <summary>
        /// Quantità disponibile a magazzino.
        /// </summary>
        [Required(ErrorMessage = "La quantità disponibile è obbligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore o uguale a zero.")]
        public int QuantitaDisponibile { get; set; }
    }
}
