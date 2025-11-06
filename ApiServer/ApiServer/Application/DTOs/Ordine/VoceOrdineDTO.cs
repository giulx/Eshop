namespace ApiServer.Application.DTOs.Ordine
{
    // DTO per leggere le informazioni di una singola voce dell'ordine
    public class VoceOrdineDTO
    {
        public int ProdottoId { get; set; }
        public string NomeProdotto { get; set; } = string.Empty;
        public int Quantita { get; set; }
        public decimal PrezzoUnitario { get; set; }
    }
}
