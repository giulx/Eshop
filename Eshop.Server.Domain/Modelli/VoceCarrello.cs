using System;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Domain.Modelli
{
    /// <summary>
    /// Riga del carrello, con snapshot del prezzo unitario al momento dell’aggiunta/aggiornamento.
    /// </summary>
    public class VoceCarrello
    {
        public int Id { get; private set; }

        public int CarrelloId { get; private set; }
        public Carrello Carrello { get; private set; } = null!;

        public int ProdottoId { get; private set; }
        public Prodotto Prodotto { get; private set; } = null!;

        public Quantita Quantita { get; private set; } = null!;

        /// <summary>
        /// Prezzo unitario snapshot (non live). Possiamo rinfrescarlo prima del checkout.
        /// </summary>
        public Money PrezzoUnitarioSnapshot { get; private set; } = null!;

        protected VoceCarrello() { }

        public VoceCarrello(Carrello carrello, Prodotto prodotto, Quantita quantita)
        {
            Carrello = carrello ?? throw new ArgumentNullException(nameof(carrello));
            CarrelloId = carrello.Id;

            Prodotto = prodotto ?? throw new ArgumentNullException(nameof(prodotto));
            ProdottoId = prodotto.Id;

            if (quantita is null || quantita.Valore <= 0)
                throw new ArgumentException("Quantità deve essere positiva.", nameof(quantita));

            Quantita = quantita;

            // snapshot iniziale
            PrezzoUnitarioSnapshot = new Money(prodotto.Prezzo.Valore, prodotto.Prezzo.Valuta);
        }

        public void AggiungiQuantita(Quantita delta)
        {
            if (delta is null || delta.Valore <= 0)
                throw new ArgumentException("Quantità deve essere positiva.", nameof(delta));

            Quantita = new Quantita(Quantita.Valore + delta.Valore);
        }

        public void ImpostaQuantita(Quantita nuova)
        {
            if (nuova is null || nuova.Valore <= 0)
                throw new ArgumentException("Quantità deve essere positiva.", nameof(nuova));

            Quantita = nuova;
        }

        public void AggiornaPrezzoSnapshot(Money nuovoPrezzoUnitario)
        {
            PrezzoUnitarioSnapshot = nuovoPrezzoUnitario ?? throw new ArgumentNullException(nameof(nuovoPrezzoUnitario));
        }

        /// <summary>
        /// Totale voce calcolato sugli snapshot.
        /// </summary>
        public Money TotaleSnapshot()
        {
            var valore = PrezzoUnitarioSnapshot.Valore * Quantita.Valore;
            return new Money(valore, PrezzoUnitarioSnapshot.Valuta);
        }
    }
}
