using System;

namespace Eshop.Server.Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        public string Value { get; private set; } = string.Empty;

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("L'email non può essere vuota.", nameof(value));

            var normalized = value.Trim();

            if (!normalized.Contains("@"))
                throw new ArgumentException("Email non valida.", nameof(value));

            // NORMALIZZO SEMPRE
            Value = normalized.ToLowerInvariant();
        }

        protected Email() { }

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            // adesso sono già normalizzate, ma teniamolo robusto
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj) => Equals(obj as Email);

        public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

        public override string ToString() => Value;
    }
}
