using System;
using Eshop.Server.Domain.Entities;

namespace Eshop.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che rappresenta la creazione di un new order.
    /// Può essere utilizzato per attivare logiche di notifica, fatturazione o aggiornamento stock.
    /// </summary>
    public sealed class OrderCreatedEvent
    {
        public Order Order { get; }
        public DateTime DataEvento { get; }

        public OrderCreatedEvent(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            DataEvento = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[{DataEvento:yyyy-MM-dd HH:mm:ss}] Creato order #{Order.Id} per {Order.Customer.Email}";
    }
}
