using System;
using ApiServer.Domain.Models;
using ApiServer.Domain.ValueObjects;

namespace Ecommerce.Domain.Events
{
    public class IndirizzoAggiornato
    {
        public Utente Utente { get; }
        public Indirizzo NuovoIndirizzo { get; }
        public DateTime DataEvento { get; }

        public IndirizzoAggiornato(Utente utente, Indirizzo nuovoIndirizzo)
        {
            Utente = utente ?? throw new ArgumentNullException(nameof(utente));
            NuovoIndirizzo = nuovoIndirizzo ?? throw new ArgumentNullException(nameof(nuovoIndirizzo));
            DataEvento = DateTime.UtcNow;
        }
    }
}
