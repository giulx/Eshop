using System;
using ApiServer.Domain.ValueObjects;

namespace ApiServer.Domain.Models
{
    /// <summary>
    /// Entità Prodotto nel dominio.
    /// </summary>
    public class Prodotto
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Descrizione { get; private set; } = string.Empty;
        public Money Prezzo { get; private set; }
        public int QuantitaDisponibile { get; private set; }

        protected Prodotto() { } // EF / ORM friendly

        public Prodotto(string nome, string descrizione, Money prezzo, int quantitaDisponibile)
        {
            if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome non valido.", nameof(nome));
            if (prezzo == null) throw new ArgumentNullException(nameof(prezzo));
            if (quantitaDisponibile < 0) throw new ArgumentException("Quantità non valida.", nameof(quantitaDisponibile));

            Nome = nome;
            Descrizione = descrizione;
            Prezzo = prezzo;
            QuantitaDisponibile = quantitaDisponibile;
        }

        public void AggiornaQuantita(int nuovaQuantita)
        {
            if (nuovaQuantita < 0) throw new ArgumentException("Quantità non valida.", nameof(nuovaQuantita));
            QuantitaDisponibile = nuovaQuantita;
        }
    }
}


