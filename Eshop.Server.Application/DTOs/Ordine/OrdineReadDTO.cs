using Eshop.Server.Domain.Modelli;

namespace Eshop.Server.Application.DTOs.Ordine
{
    /// <summary>
    /// DTO di lettura per un ordine esistente.
    /// </summary>
    public class OrdineReadDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataCreazione { get; set; }

        /// <summary>
        /// Stato dell'ordine (es. CREATED, PAID, CANCELLED).
        /// </summary>
        public StatoOrdine Stato { get; set; } = 0;

        /// <summary>
        /// Totale complessivo dell'ordine.
        /// </summary>
        public decimal Totale { get; set; }

        /// <summary>
        /// Voci dell'ordine (prodotti acquistati).
        /// </summary>
        public List<VoceOrdineDTO> Voci { get; set; } = new();
    }
}
