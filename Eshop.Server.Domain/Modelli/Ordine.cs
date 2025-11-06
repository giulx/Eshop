using System;
using System.Collections.Generic;
using System.Linq;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Domain.Modelli
{
    /// <summary>
    /// Stati possibili per un ordine.
    /// </summary>
    public enum StatoOrdine
    {
        /// <summary>
        /// Ordine creato e pagato dal cliente.
        /// Da qui può essere messo in elaborazione o cancellato.
        /// </summary>
        Pagato = 0,

        /// <summary>
        /// Ordine preso in carico dal sistema / magazzino.
        /// Da qui non può più essere cancellato dal cliente.
        /// </summary>
        InElaborazione = 1,

        /// <summary>
        /// Ordine spedito al cliente.
        /// </summary>
        Spedito = 2,

        /// <summary>
        /// Ordine annullato (possibile solo quando era ancora pagato).
        /// </summary>
        Cancellato = 3
    }

    /// <summary>
    /// Aggregato Ordine.
    /// Rappresenta un acquisto confermato da un cliente.
    /// Lo snapshot dei prodotti è nelle voci.
    /// </summary>
    public class Ordine
    {
        public int Id { get; private set; }

        public int ClienteId { get; private set; }
        public Utente? Cliente { get; private set; }

        /// <summary>
        /// Data e ora di creazione dell'ordine (UTC).
        /// </summary>
        public DateTime DataCreazione { get; private set; }

        /// <summary>
        /// Stato attuale dell'ordine.
        /// </summary>
        public StatoOrdine Stato { get; private set; }

        /// <summary>
        /// Voci dell’ordine (snapshot dei prodotti al momento dell’ordine).
        /// </summary>
        public List<VoceOrdine> Voci { get; private set; } = new();

        // EF
        protected Ordine() { }

        /// <summary>
        /// Costruisce un ordine per un cliente già caricato.
        /// Il service dovrebbe chiamare questo costruttore SOLO dopo che il pagamento è ok,
        /// quindi partiamo direttamente da stato = Pagato.
        /// </summary>
        public Ordine(Utente cliente)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            ClienteId = cliente.Id;
            DataCreazione = DateTime.UtcNow;
            Stato = StatoOrdine.Pagato;
        }

        /// <summary>
        /// Costruisce un ordine avendo solo l'id del cliente.
        /// </summary>
        public Ordine(int clienteId)
        {
            ClienteId = clienteId;
            DataCreazione = DateTime.UtcNow;
            Stato = StatoOrdine.Pagato;
        }

        /// <summary>
        /// Aggiunge una voce all'ordine usando i dati "fotografati".
        /// Nessun controllo di magazzino qui dentro.
        /// </summary>
        public void AggiungiVoce(
            int prodottoId,
            string nomeProdotto,
            Money prezzoUnitario,
            int quantita)
        {
            if (quantita <= 0)
                throw new ArgumentException("La quantità deve essere positiva.", nameof(quantita));

            Voci.Add(new VoceOrdine(
                ordine: this,
                prodottoId: prodottoId,
                nomeProdotto: nomeProdotto,
                prezzoUnitario: prezzoUnitario,
                quantita: quantita
            ));
        }

        /// <summary>
        /// Calcola il totale dell’ordine sommando le voci.
        /// </summary>
        public Money CalcolaTotale()
        {
            if (Voci.Count == 0)
                return new Money(0m, "EUR");

            var valuta = Voci[0].PrezzoUnitario.Valuta;
            var somma = Voci.Sum(v => v.Subtotale.Valore);
            return new Money(somma, valuta);
        }

        // ========================
        // Gestione stato
        // ========================

        /// <summary>
        /// Porta l'ordine in stato "In elaborazione".
        /// Consentito solo se l'ordine è pagato.
        /// </summary>
        public void ImpostaInElaborazione()
        {
            if (Stato != StatoOrdine.Pagato)
                throw new InvalidOperationException("Puoi mettere in elaborazione solo un ordine pagato.");

            Stato = StatoOrdine.InElaborazione;
        }

        /// <summary>
        /// Marca l'ordine come spedito.
        /// Consentito solo se è in elaborazione.
        /// </summary>
        public void ImpostaSpedito()
        {
            if (Stato != StatoOrdine.InElaborazione)
                throw new InvalidOperationException("Puoi spedire solo un ordine in elaborazione.");

            Stato = StatoOrdine.Spedito;
        }

        /// <summary>
        /// Annulla l'ordine.
        /// Consentito solo se è ancora pagato (non già in lavorazione o spedito).
        /// </summary>
        public void Annulla()
        {
            if (Stato != StatoOrdine.Pagato)
                throw new InvalidOperationException("Puoi cancellare solo un ordine pagato e non ancora elaborato.");

            Stato = StatoOrdine.Cancellato;
        }

        public void ForceCancel()
        {
            Stato = StatoOrdine.Cancellato;
        }

        // ========================
        // Metodi per ADMIN
        // ========================

        /// <summary>
        /// Permette all'admin di impostare direttamente lo stato "In elaborazione".
        /// </summary>
        public void ImpostaInElaborazioneDaAdmin()
        {
            Stato = StatoOrdine.InElaborazione;
        }

        /// <summary>
        /// Permette all'admin di impostare direttamente lo stato "Spedito".
        /// </summary>
        public void ImpostaSpeditoDaAdmin()
        {
            Stato = StatoOrdine.Spedito;
        }
    }
}
