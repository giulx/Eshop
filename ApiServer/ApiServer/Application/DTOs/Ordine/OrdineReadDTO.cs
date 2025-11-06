namespace ApiServer.Application.DTOs.Ordine
{
    public class OrdineReadDTO
    {
        public int Id { get; set; }
        public int UtenteId { get; set; }
        public DateTime DataOrdine { get; set; }
        public decimal Totale { get; set; }
        public List<VoceOrdineDTO> Articoli { get; set; } = new();
    }
}
