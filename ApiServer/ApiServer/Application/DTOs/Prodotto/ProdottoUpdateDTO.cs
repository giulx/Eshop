namespace ApiServer.Application.DTOs.Prodotto
{
    // DTO per aggiornare un prodotto
    public class ProdottoUpdateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public decimal Prezzo { get; set; }
        public int QuantitaDisponibile { get; set; }
    }
}
