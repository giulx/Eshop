using System;

namespace Eshop.Server.Dominio.OggettiValore
{
    /// <summary>
    /// Value Object che rappresenta un indirizzo email valido.
    /// </summary>
    public class Email : IEquatable<Email>
    {
        public string Valore { get; private set; } = string.Empty;

        public Email(string valore)
        {
            if (string.IsNullOrWhiteSpace(valore))
                throw new ArgumentException("L'email non può essere vuota.", nameof(valore));

            if (!valore.Contains("@"))
                throw new ArgumentException("Email non valida.", nameof(valore));

            Valore = valore;
        }

        // Costruttore per EF Core
        protected Email() { }

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            return string.Equals(Valore, other.Valore, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj) => Equals(obj as Email);

        public override int GetHashCode() => Valore.ToLowerInvariant().GetHashCode();

        public override string ToString() => Valore;
    }
}
