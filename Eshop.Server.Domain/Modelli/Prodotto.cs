using System;
using Eshop.Server.Dominio.OggettiValore;

namespace Eshop.Server.Dominio.Modelli
{
    // Entità del dominio che rappresenta un prodotto vendibile nel catalogo.
    public class Prodotto
    {
        public int Id { get; private set; }

        // Nome del prodotto.
        public string Nome { get; private set; } = string.Empty;

        // Descrizione testuale del prodotto.
        public string Descrizione { get; private set; } = string.Empty;

        // Prezzo unitario espresso come Value Object.
        public Money Prezzo { get; private set; }

        // Quantità disponibile in magazzino.
        public int QuantitaDisponibile { get; private set; }

        // Costruttore richiesto da Entity Framework
        protected Prodotto() { }

        // Crea un nuovo prodotto con i dati principali
        public Prodotto(string nome, string descrizione, Money prezzo, int quantitaDisponibile)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Il nome del prodotto non può essere vuoto.", nameof(nome));

            Prezzo = prezzo ?? throw new ArgumentNullException(nameof(prezzo));

            if (quantitaDisponibile < 0)
                throw new ArgumentException("La quantità disponibile non può essere negativa.", nameof(quantitaDisponibile));

            Nome = nome;
            Descrizione = descrizione ?? string.Empty;
            QuantitaDisponibile = quantitaDisponibile;
        }

        // -------------------------
        // aggiornamenti "classici"
        // -------------------------
        public void AggiornaQuantita(int nuovaQuantita)
        {
            if (nuovaQuantita < 0)
                throw new ArgumentException("La quantità non può essere negativa.", nameof(nuovaQuantita));

            QuantitaDisponibile = nuovaQuantita;
        }

        public void AggiornaPrezzo(Money nuovoPrezzo)
        {
            Prezzo = nuovoPrezzo ?? throw new ArgumentNullException(nameof(nuovoPrezzo));
        }

        public void AggiornaDescrizione(string nuovaDescrizione)
        {
            if (nuovaDescrizione == null)
                throw new ArgumentNullException(nameof(nuovaDescrizione));

            Descrizione = nuovaDescrizione.Trim();
        }

        public void AggiornaNome(string nuovoNome)
        {
            if (string.IsNullOrWhiteSpace(nuovoNome))
                throw new ArgumentException("Il nome non può essere vuoto.", nameof(nuovoNome));

            Nome = nuovoNome.Trim();
        }

        // -------------------------
        // metodi per ORDINI
        // -------------------------

        /// <summary>
        /// True se il magazzino ha almeno la quantità richiesta.
        /// </summary>
        public bool HaDisponibilita(int quantitaRichiesta)
        {
            if (quantitaRichiesta <= 0)
                return false;

            return QuantitaDisponibile >= quantitaRichiesta;
        }

        /// <summary>
        /// Scala la quantità dal magazzino.
        /// Da chiamare SOLO quando l’ordine è confermato.
        /// </summary>
        public void ScalaQuantita(int quantita)
        {
            if (quantita <= 0)
                throw new ArgumentException("La quantità da scalare dev'essere positiva.", nameof(quantita));

            if (quantita > QuantitaDisponibile)
                throw new InvalidOperationException("Quantità richiesta superiore alla disponibilità.");

            QuantitaDisponibile -= quantita;
        }
    }
}
