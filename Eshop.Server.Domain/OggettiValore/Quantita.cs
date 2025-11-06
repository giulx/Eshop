using System;

namespace Eshop.Server.Dominio.OggettiValore
{
    /// <summary>
    /// Value Object che rappresenta una quantità > 0.
    /// </summary>
    public class Quantita : IEquatable<Quantita>
    {
        public int Valore { get; private set; }

        public Quantita(int valore)
        {
            if (valore <= 0)
                throw new ArgumentException("La quantità deve essere maggiore di zero.", nameof(valore));

            Valore = valore;
        }

        // Costruttore per EF Core
        protected Quantita() { }

        public bool Equals(Quantita? other)
        {
            return other is not null && Valore == other.Valore;
        }

        public override bool Equals(object? obj) => Equals(obj as Quantita);

        public override int GetHashCode() => Valore.GetHashCode();

        public override string ToString() => Valore.ToString();
    }
}
