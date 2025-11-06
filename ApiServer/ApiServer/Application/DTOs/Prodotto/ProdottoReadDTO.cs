namespace ApiServer.Application.DTOs.Prodotto
{
	// DTO per leggere un prodotto
	public class ProdottoReadDTO
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string Descrizione { get; set; } = string.Empty;
		public decimal Prezzo { get; set; }
		public int QuantitaDisponibile { get; set; }
	}
}
