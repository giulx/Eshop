using System;

namespace Eshop.Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object che rappresenta un indirizzo di spedizione o contatto.
    /// È immutabile a livello di dominio (solo getter),
    /// ma EF Core può valorizzarlo tramite il costruttore protetto senza parametri.
    /// </summary>
    public class Address : IEquatable<Address>
    {
        public string Street { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string PostalCode { get; private set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;

        /// <summary>
        /// Costruttore principale del dominio.
        /// Gestisce eventuali campi vuoti impostando valori di fallback.
        /// </summary>
        public Address(string street, string city, string postalCode, string number)
        {
            Street = string.IsNullOrWhiteSpace(street)
                ? throw new ArgumentException("Street non può essere vuota.", nameof(street))
                : street.Trim();

            City = string.IsNullOrWhiteSpace(city)
                ? throw new ArgumentException("City non può essere vuota.", nameof(city))
                : city.Trim();

            PostalCode = string.IsNullOrWhiteSpace(postalCode)
                ? "00000" // fallback per evitare errori in fase di seeding
                : postalCode.Trim();

            Number = string.IsNullOrWhiteSpace(number)
                ? "s.n." // senza numero
                : number.Trim();
        }

        /// <summary>
        /// Costruttore senza parametri richiesto da EF Core.
        /// </summary>
        protected Address() { }

        public bool Equals(Address? other)
        {
            if (other is null) return false;

            return Street == other.Street
                && City == other.City
                && PostalCode == other.PostalCode
                && Number == other.Number;
        }

        public override bool Equals(object? obj) => Equals(obj as Address);

        public override int GetHashCode() => HashCode.Combine(Street, City, PostalCode, Number);

        public override string ToString() => $"{Street}, {Number}, {City} ({PostalCode})";
    }
}
