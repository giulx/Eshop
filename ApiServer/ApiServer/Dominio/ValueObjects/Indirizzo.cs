using System;

namespace ApiServer.Domain.ValueObjects
{
    /// <summary>
    /// Value Object per l'indirizzo.
    /// È immutabile e rappresenta dati di spedizione o contatto.
    /// </summary>
    public class Indirizzo
    {
        public string Via { get; }
        public string Citta { get; }
        public string CAP { get; }
        public string Telefono { get; }

        public Indirizzo(string via, string citta, string cap, string telefono)
        {
            if (string.IsNullOrWhiteSpace(via)) throw new ArgumentException("Via non può essere vuota.", nameof(via));
            if (string.IsNullOrWhiteSpace(citta)) throw new ArgumentException("Città non può essere vuota.", nameof(citta));
            if (string.IsNullOrWhiteSpace(cap)) throw new ArgumentException("CAP non può essere vuoto.", nameof(cap));
            if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Telefono non può essere vuoto.", nameof(telefono));

            Via = via;
            Citta = citta;
            CAP = cap;
            Telefono = telefono;
        }

        public override bool Equals(object? obj)
        {
            return obj is Indirizzo i &&
                   i.Via == Via &&
                   i.Citta == Citta &&
                   i.CAP == CAP &&
                   i.Telefono == Telefono;
        }

        public override int GetHashCode() => HashCode.Combine(Via, Citta, CAP, Telefono);

        public override string ToString() => $"{Via}, {Citta}, {CAP}, {Telefono}";
    }
}
