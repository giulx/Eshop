using System;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Events
{
    /// <summary>
    /// Evento di dominio che rappresenta l'aggiornamento dell'address di un user.
    /// Viene generato quando un user modifica il proprio address di contatto o spedizione.
    /// </summary>
    public sealed class AddressUpdatedEvent
    {
        public User User { get; }
        public Address NuovoAddress { get; }
        public DateTime DataEvento { get; }

        public AddressUpdatedEvent(User user, Address newAddress)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            NuovoAddress = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
            DataEvento = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[{DataEvento:yyyy-MM-dd HH:mm:ss}] Aggiornato address per {User.Email}: {NuovoAddress}";
    }
}
