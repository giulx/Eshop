using System;
using System.Collections.Generic;
using System.Linq;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Entities
{
    /// <summary>
    /// Stati possibili per un order.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order creato e paid dal customer.
        /// Da qui può essere messo in elaborazione o cancelled.
        /// </summary>
        Paid = 0,

        /// <summary>
        /// Order preso in carico dal sistema / magazzino.
        /// Da qui non può più essere cancelled dal customer.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Order shipped al customer.
        /// </summary>
        Shipped = 2,

        /// <summary>
        /// Order cancelto (possibile solo quando era ancora paid).
        /// </summary>
        Cancelled = 3
    }

    /// <summary>
    /// Aggregato Order.
    /// Rappresenta un acquisto confermato da un customer.
    /// Lo snapshot dei prodotti è nelle items.
    /// </summary>
    public class Order
    {
        public int Id { get; private set; }

        public int CustomerId { get; private set; }
        public User? Customer { get; private set; }

        /// <summary>
        /// Data e ora di creazione dell'order (UTC).
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Status current dell'order.
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// Items dell’order (snapshot dei prodotti al momento dell’order).
        /// </summary>
        public List<OrderItem> Items { get; private set; } = new();

        // EF
        protected Order() { }

        /// <summary>
        /// Costruisce un order per un customer già caricato.
        /// Il service dovrebbe chiamare questo costruttore SOLO dopo che il pagamento è ok,
        /// quindi partiamo direttamente da status = Paid.
        /// </summary>
        public Order(User customer)
        {
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            CustomerId = customer.Id;
            CreationDate = DateTime.UtcNow;
            Status = OrderStatus.Paid;
        }

        /// <summary>
        /// Costruisce un order avendo solo l'id del customer.
        /// </summary>
        public Order(int customerId)
        {
            CustomerId = customerId;
            CreationDate = DateTime.UtcNow;
            Status = OrderStatus.Paid;
        }

        /// <summary>
        /// Aggiunge una item all'order usando i dati "fotografati".
        /// Nessun controllo di magazzino qui dentro.
        /// </summary>
        public void AddItem(
            int productId,
            string productName,
            Money unitPrice,
            int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La quantity deve essere positiva.", nameof(quantity));

            Items.Add(new OrderItem(
                order: this,
                productId: productId,
                productName: productName,
                unitPrice: unitPrice,
                quantity: quantity
            ));
        }

        /// <summary>
        /// Calcola il totale dell’order sumndo le items.
        /// </summary>
        public Money CalculateTotal()
        {
            if (Items.Count == 0)
                return new Money(0m, "EUR");

            var currency = Items[0].UnitPrice.Currency;
            var sum = Items.Sum(v => v.Subtotal.Value);
            return new Money(sum, currency);
        }

        // ========================
        // Gestione status
        // ========================

        /// <summary>
        /// Porta l'order in status "In elaborazione".
        /// Consentito solo se l'order è paid.
        /// </summary>
        public void MarkAsProcessing()
        {
            if (Status != OrderStatus.Paid)
                throw new InvalidOperationException("Puoi mettere in elaborazione solo un order paid.");

            Status = OrderStatus.Processing;
        }

        /// <summary>
        /// Marca l'order come shipped.
        /// Consentito solo se è in elaborazione.
        /// </summary>
        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException("Puoi spedire solo un order in elaborazione.");

            Status = OrderStatus.Shipped;
        }

        /// <summary>
        /// Cancel l'order.
        /// Consentito solo se è ancora paid (non già in lavorazione o shipped).
        /// </summary>
        public void Cancel()
        {
            if (Status != OrderStatus.Paid)
                throw new InvalidOperationException("Puoi cancellare solo un order paid e non ancora elaborato.");

            Status = OrderStatus.Cancelled;
        }

        public void ForceCancel()
        {
            Status = OrderStatus.Cancelled;
        }

        // ========================
        // Metodi per ADMIN
        // ========================

        /// <summary>
        /// Permette all'admin di impostare direttamente lo status "In elaborazione".
        /// </summary>
        public void MarkAsProcessingByAdmin()
        {
            Status = OrderStatus.Processing;
        }

        /// <summary>
        /// Permette all'admin di impostare direttamente lo status "Shipped".
        /// </summary>
        public void MarkAsShippedByAdmin()
        {
            Status = OrderStatus.Shipped;
        }
    }
}
