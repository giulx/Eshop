using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Entities
{
    /// <summary>
    /// Riga di un order. Contiene lo snapshot del product al momento dell’order.
    /// </summary>
    public class OrderItem
    {
        public int Id { get; private set; }

        public int OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public int ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;

        public Money UnitPrice { get; private set; }

        public int Quantity { get; private set; }

        public Money Subtotal => new Money(UnitPrice.Value * Quantity, UnitPrice.Currency);

        protected OrderItem() { }

        public OrderItem(Order order, int productId, string productName, Money unitPrice, int quantity)
        {
            Order = order;
            OrderId = order.Id;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
