using System;

namespace ApiServer.Domain.ValueObjects
{
    /// <summary>
    /// Value Object per rappresentare quantità ordinata.
    /// Garantisce valori validi (> 0)
    /// </summary>
    public class Quantita : IEquatable<Quantita>
    {
        public int Valore { get; }

        public Quantita(int valore)
        {
            if (valore <= 0) throw new ArgumentException("Quantità deve essere maggiore di zero.", nameof(valore));
            Valore = valore;
        }

        public bool Equals(Quantita? other) => other != null && Valore == other.Valore;
        public override bool Equals(object? obj) => Equals(obj as Quantita);
        public override int GetHashCode() => Valore.GetHashCode();

        public override string ToString() => Valore.ToString();
    }
}
