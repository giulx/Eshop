using System;
using System.Collections.Generic;
using System.Linq;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Domain.Modelli
{
    /// <summary>
    /// Aggregato Carrello: collezione di voci non vincolanti (niente stock lock).
    /// Lo stock e i prezzi vengono verificati al checkout (creazione ordine).
    /// </summary>
    public class Carrello
    {
        public int Id { get; private set; }

        /// <summary>
        /// FK verso il cliente proprietario del carrello.
        /// </summary>
        public int ClienteId { get; private set; }

        /// <summary>
        /// Navigazione verso l'utente.
        /// </summary>
        public Utente Cliente { get; private set; } = null!;

        /// <summary>
        /// Voci del carrello.
        /// </summary>
        public List<VoceCarrello> Voci { get; private set; } = new();

        /// <summary>
        /// Ultimo momento in cui i prezzi snapshot sono stati aggiornati.
        /// </summary>
        public DateTime UltimoAggiornamentoPrezziUtc { get; private set; } = DateTime.UtcNow;

        protected Carrello() { }

        public Carrello(Utente cliente)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            ClienteId = cliente.Id;
        }

        /// <summary>
        /// Aggiunge una voce (o aumenta quantità se già presente).
        /// NON scala stock. Aggiorna lo snapshot prezzo con il prezzo attuale del prodotto.
        /// </summary>
        public void AggiungiOIncrementa(Prodotto prodotto, Quantita quantita)
        {
            if (prodotto is null) throw new ArgumentNullException(nameof(prodotto));
            if (quantita is null) throw new ArgumentNullException(nameof(quantita));
            if (quantita.Valore <= 0) throw new ArgumentException("Quantità deve essere positiva.", nameof(quantita));

            var esistente = Voci.FirstOrDefault(v => v.ProdottoId == prodotto.Id);
            if (esistente is null)
            {
                Voci.Add(new VoceCarrello(this, prodotto, quantita));
            }
            else
            {
                esistente.AggiungiQuantita(quantita);
                esistente.AggiornaPrezzoSnapshot(prodotto.Prezzo);
            }

            UltimoAggiornamentoPrezziUtc = DateTime.UtcNow;
        }

        public void AggiornaQuantita(int prodottoId, Quantita nuovaQuantita)
        {
            var voce = Voci.FirstOrDefault(v => v.ProdottoId == prodottoId)
                       ?? throw new InvalidOperationException("Prodotto non presente nel carrello.");

            if (nuovaQuantita.Valore <= 0)
            {
                Voci.Remove(voce);
            }
            else
            {
                voce.ImpostaQuantita(nuovaQuantita);
            }

            UltimoAggiornamentoPrezziUtc = DateTime.UtcNow;
        }

        public void RimuoviVoce(int prodottoId)
        {
            var voce = Voci.FirstOrDefault(v => v.ProdottoId == prodottoId);
            if (voce != null)
            {
                Voci.Remove(voce);
                UltimoAggiornamentoPrezziUtc = DateTime.UtcNow;
            }
        }

        public void Svuota()
        {
            Voci.Clear();
            UltimoAggiornamentoPrezziUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcola il totale usando gli snapshot (non i prezzi live).
        /// </summary>
        public Money CalcolaTotaleSnapshot()
        {
            if (Voci.Count == 0) return new Money(0m, "EUR");
            var valuta = Voci[0].PrezzoUnitarioSnapshot.Valuta;
            var somma = Voci.Sum(v => v.PrezzoUnitarioSnapshot.Valore * v.Quantita.Valore);
            return new Money(somma, valuta);
        }

        /// <summary>
        /// Da chiamare quando aggiorniamo tutti gli snapshot prezzi (batch refresh).
        /// </summary>
        public void MarcaPrezziAggiornatiOra() => UltimoAggiornamentoPrezziUtc = DateTime.UtcNow;
    }
}
