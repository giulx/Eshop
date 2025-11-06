namespace ApiServer.Application.DTOs.Ordine
{
    public class OrdineCreateDTO
    {
        public int UtenteId { get; set; }
        public List<VoceOrdineCreateDTO> Articoli { get; set; } = new();
    }
}
