namespace ApiServer.Application.DTOs.Prodotto
{
    // DTO per creare un prodotto
    public class ProdottoCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public decimal Prezzo { get; set; }
        public int QuantitaDisponibile { get; set; }
    }
}
