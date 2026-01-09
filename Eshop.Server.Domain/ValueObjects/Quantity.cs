using System;

namespace Eshop.Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object che rappresenta una quantity > 0.
    /// </summary>
    public class Quantity : IEquatable<Quantity>
    {
        public int Value { get; private set; }

        public Quantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("La quantity deve essere maggiore di zero.", nameof(value));

            Value = value;
        }

        // Costruttore per EF Core
        protected Quantity() { }

        public bool Equals(Quantity? other)
        {
            return other is not null && Value == other.Value;
        }

        public override bool Equals(object? obj) => Equals(obj as Quantity);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }
}
