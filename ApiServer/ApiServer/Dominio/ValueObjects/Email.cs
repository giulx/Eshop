using System;

namespace ApiServer.Domain.ValueObjects
{
    /// <summary>
    /// Value Object che rappresenta un'email valida.
    /// </summary>
    public class Email
    {
        public string Valore { get; }

        public Email(string valore)
        {
            if (string.IsNullOrWhiteSpace(valore))
                throw new ArgumentException("L'email non può essere vuota.", nameof(valore));

            if (!valore.Contains("@"))
                throw new ArgumentException("Email non valida.", nameof(valore));

            Valore = valore;
        }

        // Equality basata sul valore
        public override bool Equals(object? obj)
        {
            if (obj is not Email other) return false;
            return Valore.Equals(other.Valore, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => Valore.ToLowerInvariant().GetHashCode();

        public override string ToString() => Valore;
    }
}
