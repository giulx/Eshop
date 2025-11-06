using System;
using System.Collections.Generic;
using System.Linq;
using Eshop.Server.Dominio.OggettiValore;

namespace Eshop.Server.Dominio.Modelli
{
    /// <summary>
    /// Entità Ordine nel dominio.
    /// Rappresenta un acquisto effettuato da un cliente, composto da più voci d'ordine.
    /// </summary>
    public class Ordine
    {
        public int Id { get; private set; }

        // Cliente che ha effettuato l'ordine
        public Utente Cliente { get; private set; }

        // Data di creazione (UTC)
        public DateTime DataCreazione { get; private set; }

        // Collezione di voci d’ordine
        public List<VoceOrdine> Voci { get; private set; } = new();

        // Stato pagamento
        public bool Pagato { get; private set; } = false;

        // Costruttore per EF Core
        protected Ordine() { }

        // Costruttore principale
        public Ordine(Utente cliente)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            DataCreazione = DateTime.UtcNow;
        }

        /// <summary>
        /// Aggiunge una voce all'ordine per un determinato prodotto e quantità.
        /// </summary>
        public void AggiungiVoce(Prodotto prodotto, Quantita quantita)
        {
            if (prodotto == null) throw new ArgumentNullException(nameof(prodotto));
            if (quantita.Valore <= 0) throw new ArgumentException("La quantità deve essere positiva.", nameof(quantita));

            Voci.Add(new VoceOrdine(prodotto, quantita));
        }

        /// <summary>
        /// Calcola il totale dell'ordine sommando il totale di ogni voce.
        /// </summary>
        public Money CalcolaTotale()
        {
            return Voci
                .Select(v => v.Totale)
                .Aggregate(new Money(0m), (acc, valore) => acc.Aggiungi(valore));
        }

        /// <summary>
        /// Segna l'ordine come pagato.
        /// </summary>
        public void MarcaComePagato()
        {
            Pagato = true;
        }
    }
}
