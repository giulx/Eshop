using System;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Entities
{
    /// <summary>
    /// Riga del cart, con snapshot del price unitario al momento dell’aggiunta/aggiornamento.
    /// </summary>
    public class CartItem
    {
        public int Id { get; private set; }

        public int CartId { get; private set; }
        public Cart Cart { get; private set; } = null!;

        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;

        public Quantity Quantity { get; private set; } = null!;

        /// <summary>
        /// Price unitario snapshot (non live). Possiamo rinfrescarlo prima del checkout.
        /// </summary>
        public Money UnitPriceSnapshot { get; private set; } = null!;

        protected CartItem() { }

        public CartItem(Cart cart, Product product, Quantity quantity)
        {
            Cart = cart ?? throw new ArgumentNullException(nameof(cart));
            CartId = cart.Id;

            Product = product ?? throw new ArgumentNullException(nameof(product));
            ProductId = product.Id;

            if (quantity is null || quantity.Value <= 0)
                throw new ArgumentException("Quantity deve essere positiva.", nameof(quantity));

            Quantity = quantity;

            // snapshot iniziale
            UnitPriceSnapshot = new Money(product.Price.Value, product.Price.Currency);
        }

        public void AddQuantity(Quantity delta)
        {
            if (delta is null || delta.Value <= 0)
                throw new ArgumentException("Quantity deve essere positiva.", nameof(delta));

            Quantity = new Quantity(Quantity.Value + delta.Value);
        }

        public void SetQuantity(Quantity newQuantity)
        {
            if (newQuantity is null || newQuantity.Value <= 0)
                throw new ArgumentException("Quantity deve essere positiva.", nameof(newQuantity));

            Quantity = newQuantity;
        }

        public void UpdatePriceSnapshot(Money newUnitPrice)
        {
            UnitPriceSnapshot = newUnitPrice ?? throw new ArgumentNullException(nameof(newUnitPrice));
        }

        /// <summary>
        /// Total item calcolato sugli snapshot.
        /// </summary>
        public Money TotalSnapshot()
        {
            var value = UnitPriceSnapshot.Value * Quantity.Value;
            return new Money(value, UnitPriceSnapshot.Currency);
        }
    }
}
