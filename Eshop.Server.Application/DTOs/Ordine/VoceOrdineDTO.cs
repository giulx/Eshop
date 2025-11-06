namespace Eshop.Server.Applicazione.DTOs.Ordine
{
    /// <summary>
    /// DTO per una voce dell'ordine (singolo prodotto acquistato).
    /// </summary>
    public class VoceOrdineDTO
    {
        public int ProdottoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal PrezzoUnitario { get; set; }
        public int Quantita { get; set; }
        public decimal Subtotale { get; set; }
    }
}
