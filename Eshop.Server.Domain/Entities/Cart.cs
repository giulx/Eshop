using System;
using System.Collections.Generic;
using System.Linq;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Entities
{
    /// <summary>
    /// Aggregato Cart: collezione di items non vincolanti (niente stock lock).
    /// Lo stock e i prezzi vengono verificati al checkout (creazione order).
    /// </summary>
    public class Cart
    {
        public int Id { get; private set; }

        /// <summary>
        /// FK verso il customer proprietario del cart.
        /// </summary>
        public int CustomerId { get; private set; }

        /// <summary>
        /// Navigazione verso l'user.
        /// </summary>
        public User Customer { get; private set; } = null!;

        /// <summary>
        /// Items del cart.
        /// </summary>
        public List<CartItem> Items { get; private set; } = new();

        /// <summary>
        /// Ultimo momento in cui i prezzi snapshot sono stati aggiornati.
        /// </summary>
        public DateTime LastPriceUpdateUtc { get; private set; } = DateTime.UtcNow;

        protected Cart() { }

        public Cart(User customer)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            CustomerId = customer.Id;
        }

        /// <summary>
        /// Aggiunge una item (o aumenta quantity se già presente).
        /// NON scala stock. Aggiorna lo snapshot price con il price current del product.
        /// </summary>
        public void AddOrIncrease(Product product, Quantity quantity)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (quantity is null) throw new ArgumentNullException(nameof(quantity));
            if (quantity.Value <= 0) throw new ArgumentException("Quantity deve essere positiva.", nameof(quantity));

            var existing = Items.FirstOrDefault(v => v.ProductId == product.Id);
            if (existing is null)
            {
                Items.Add(new CartItem(this, product, quantity));
            }
            else
            {
                existing.AddQuantity(quantity);
                existing.UpdatePriceSnapshot(product.Price);
            }

            LastPriceUpdateUtc = DateTime.UtcNow;
        }

        public void UpdateQuantity(int productId, Quantity newQuantity)
        {
            var item = Items.FirstOrDefault(v => v.ProductId == productId)
                       ?? throw new InvalidOperationException("Product non presente nel cart.");

            if (newQuantity.Value <= 0)
            {
                Items.Remove(item);
            }
            else
            {
                item.SetQuantity(newQuantity);
            }

            LastPriceUpdateUtc = DateTime.UtcNow;
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(v => v.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
                LastPriceUpdateUtc = DateTime.UtcNow;
            }
        }

        public void Clear()
        {
            Items.Clear();
            LastPriceUpdateUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcola il totale usando gli snapshot (non i prezzi live).
        /// </summary>
        public Money CalculateTotalSnapshot()
        {
            if (Items.Count == 0) return new Money(0m, "EUR");
            var currency = Items[0].UnitPriceSnapshot.Currency;
            var sum = Items.Sum(v => v.UnitPriceSnapshot.Value * v.Quantity.Value);
            return new Money(sum, currency);
        }

        /// <summary>
        /// Da chiamare quando aggiorniamo tutti gli snapshot prezzi (batch refresh).
        /// </summary>
        public void MarkPricesUpdatedNow() => LastPriceUpdateUtc = DateTime.UtcNow;
    }
}
