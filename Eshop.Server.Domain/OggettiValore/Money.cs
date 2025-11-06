using System;

namespace Eshop.Server.Domain.OggettiValore
{
    /// <summary>
    /// Value Object che rappresenta un importo monetario con valuta.
    /// </summary>
    public class Money : IEquatable<Money>
    {
        public decimal Valore { get; private set; }
        public string Valuta { get; private set; } = "EUR";

        public Money(decimal valore, string valuta = "EUR")
        {
            if (valore < 0)
                throw new ArgumentException("Il valore monetario non può essere negativo.", nameof(valore));
            if (string.IsNullOrWhiteSpace(valuta))
                throw new ArgumentException("La valuta è obbligatoria.", nameof(valuta));

            Valore = valore;
            Valuta = valuta;
        }

        // Costruttore per EF Core
        protected Money() { }

        public Money MoltiplicaPer(int quantita)
        {
            if (quantita < 0)
                throw new ArgumentException("Quantità negativa non valida.", nameof(quantita));

            return new Money(Valore * quantita, Valuta);
        }

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Valore == other.Valore && Valuta == other.Valuta;
        }

        public override bool Equals(object? obj) => Equals(obj as Money);

        public override int GetHashCode() => HashCode.Combine(Valore, Valuta);

        public override string ToString() => $"{Valore} {Valuta}";
    }
}
