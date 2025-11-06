using System;

namespace Eshop.Server.Dominio.OggettiValore
{
    /// <summary>
    /// Value Object che rappresenta un indirizzo di spedizione / contatto.
    /// È immutabile dal punto di vista del dominio (niente setter pubblici),
    /// ma EF Core può valorizzarlo grazie al costruttore protetto senza parametri.
    /// </summary>
    public class Indirizzo : IEquatable<Indirizzo>
    {
        public string Via { get; private set; } = string.Empty;
        public string Citta { get; private set; } = string.Empty;
        public string CAP { get; private set; } = string.Empty;
        public string Telefono { get; private set; } = string.Empty;

        // Costruttore usato dal dominio
        public Indirizzo(string via, string citta, string cap, string telefono)
        {
            if (string.IsNullOrWhiteSpace(via))
                throw new ArgumentException("Via non può essere vuota.", nameof(via));
            if (string.IsNullOrWhiteSpace(citta))
                throw new ArgumentException("Città non può essere vuota.", nameof(citta));
            if (string.IsNullOrWhiteSpace(cap))
                throw new ArgumentException("CAP non può essere vuoto.", nameof(cap));
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("Telefono non può essere vuoto.", nameof(telefono));

            Via = via;
            Citta = citta;
            CAP = cap;
            Telefono = telefono;
        }

        // Costruttore senza parametri richiesto da EF Core per fare materializzazione
        protected Indirizzo() { }

        public Indirizzo(string via, string citta, string cap)
        {
            Via = via;
            Citta = citta;
            CAP = cap;
        }

        public bool Equals(Indirizzo? other)
        {
            if (other is null) return false;
            return Via == other.Via
                && Citta == other.Citta
                && CAP == other.CAP
                && Telefono == other.Telefono;
        }

        public override bool Equals(object? obj) => Equals(obj as Indirizzo);

        public override int GetHashCode() => HashCode.Combine(Via, Citta, CAP, Telefono);

        public override string ToString() => $"{Via}, {Citta}, {CAP}, {Telefono}";
    }
}
