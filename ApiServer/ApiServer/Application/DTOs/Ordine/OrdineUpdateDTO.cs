namespace ApiServer.Application.DTOs.Ordine
{
    public class OrdineUpdateDTO
    {
        // Permette solo di modificare le voci dell'ordine
        public List<VoceOrdineCreateDTO> Articoli { get; set; } = new();
    }
}
