namespace Eshop.Server.Application.DTOs.Product
{
    /// <summary>
    /// DTO per la lettura dei dati di un product.
    /// Utilizzato come risposta dai servizi applicativi e dai controller.
    /// </summary>
    public class ProductReadDTO
    {
        /// <summary>
        /// Identificativo univoco del product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name commerciale del product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description testuale o scheda informativa del product.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Price unitario corrente del product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Codice della currency (es. "EUR", "USD").
        /// </summary>
        public string Currency { get; set; } = "EUR";

        /// <summary>
        /// Quantity disponibile a magazzino.
        /// </summary>
        public int AvailableQuantity { get; set; }
    }
}
