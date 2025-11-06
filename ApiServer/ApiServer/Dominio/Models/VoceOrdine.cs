using ApiServer.Domain.ValueObjects;

namespace ApiServer.Domain.Models
{
    /// <summary>
    /// Voce di un ordine: rappresenta un prodotto e la quantità ordinata.
    /// </summary>
    public class VoceOrdine
    {
        public int Id { get; private set; }
        public Prodotto Prodotto { get; private set; }
        public Quantita Quantita { get; private set; }
        public Money Totale => Prodotto.Prezzo.MoltiplicaPer(Quantita.Valore);

        protected VoceOrdine() { } // EF / ORM

        public VoceOrdine(Prodotto prodotto, Quantita quantita)
        {
            Prodotto = prodotto ?? throw new ArgumentNullException(nameof(prodotto));
            Quantita = quantita ?? throw new ArgumentNullException(nameof(quantita));
        }
    }
}
