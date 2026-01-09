using Eshop.Server.Domain.Entities;

namespace Eshop.Server.Application.DTOs.Order
{
    /// <summary>
    /// DTO di lettura per un order existing.
    /// </summary>
    public class OrderReadDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Status dell'order (es. CREATED, PAID, CANCELLED).
        /// </summary>
        public OrderStatus Status { get; set; } = 0;

        /// <summary>
        /// Total complessivo dell'order.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Items dell'order (prodotti acquistati).
        /// </summary>
        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
