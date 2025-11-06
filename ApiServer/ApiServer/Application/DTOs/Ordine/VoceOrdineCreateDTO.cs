namespace ApiServer.Application.DTOs.Ordine
{
    // DTO per creare o aggiornare una voce dell'ordine
    public class VoceOrdineCreateDTO
    {
        public int ProdottoId { get; set; }
        public int Quantita { get; set; }
    }
}
