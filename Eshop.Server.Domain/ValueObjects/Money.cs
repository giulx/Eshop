using System;

namespace Eshop.Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object che rappresenta un importo monetario con currency.
    /// </summary>
    public class Money : IEquatable<Money>
    {
        public decimal Value { get; private set; }
        public string Currency { get; private set; } = "EUR";

        public Money(decimal value, string currency = "EUR")
        {
            if (value < 0)
                throw new ArgumentException("Il value monetario non può essere negativo.", nameof(value));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("La currency è obbligatoria.", nameof(currency));

            Value = value;
            Currency = currency;
        }

        // Costruttore per EF Core
        protected Money() { }

        public Money MultiplyBy(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity negativa non valida.", nameof(quantity));

            return new Money(Value * quantity, Currency);
        }

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Value == other.Value && Currency == other.Currency;
        }

        public override bool Equals(object? obj) => Equals(obj as Money);

        public override int GetHashCode() => HashCode.Combine(Value, Currency);

        public override string ToString() => $"{Value} {Currency}";
    }
}
