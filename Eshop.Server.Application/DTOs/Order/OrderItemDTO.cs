namespace Eshop.Server.Application.DTOs.Order
{
    /// <summary>
    /// DTO per una item dell'order (singolo product acquistatus).
    /// </summary>
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
