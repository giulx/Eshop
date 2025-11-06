using System;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Domain.Eventi
{
    /// <summary>
    /// Evento di dominio che rappresenta l'aggiornamento dell'indirizzo di un utente.
    /// Viene generato quando un utente modifica il proprio indirizzo di contatto o spedizione.
    /// </summary>
    public sealed class IndirizzoAggiornato
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

        public override string ToString() =>
            $"[{DataEvento:yyyy-MM-dd HH:mm:ss}] Aggiornato indirizzo per {Utente.Email}: {NuovoIndirizzo}";
    }
}
