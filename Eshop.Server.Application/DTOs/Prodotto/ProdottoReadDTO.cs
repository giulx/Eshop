namespace Eshop.Server.Application.DTOs.Prodotto
{
    /// <summary>
    /// DTO per la lettura dei dati di un prodotto.
    /// Utilizzato come risposta dai servizi applicativi e dai controller.
    /// </summary>
    public class ProdottoReadDTO
    {
        /// <summary>
        /// Identificativo univoco del prodotto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome commerciale del prodotto.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione testuale o scheda informativa del prodotto.
        /// </summary>
        public string? Descrizione { get; set; }

        /// <summary>
        /// Prezzo unitario corrente del prodotto.
        /// </summary>
        public decimal Prezzo { get; set; }

        /// <summary>
        /// Codice della valuta (es. "EUR", "USD").
        /// </summary>
        public string Valuta { get; set; } = "EUR";

        /// <summary>
        /// Quantità disponibile a magazzino.
        /// </summary>
        public int QuantitaDisponibile { get; set; }
    }
}
