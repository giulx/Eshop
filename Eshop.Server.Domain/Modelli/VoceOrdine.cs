using Eshop.Server.Dominio.OggettiValore;

namespace Eshop.Server.Dominio.Modelli
{
    /// <summary>
    /// Riga di un ordine. Contiene lo snapshot del prodotto al momento dell’ordine.
    /// </summary>
    public class VoceOrdine
    {
        public int Id { get; private set; }

        public int OrdineId { get; private set; }
        public Ordine Ordine { get; private set; } = null!;

        public int ProdottoId { get; private set; }
        public string NomeProdotto { get; private set; } = string.Empty;

        public Money PrezzoUnitario { get; private set; }

        public int Quantita { get; private set; }

        public Money Subtotale => new Money(PrezzoUnitario.Valore * Quantita, PrezzoUnitario.Valuta);

        protected VoceOrdine() { }

        public VoceOrdine(Ordine ordine, int prodottoId, string nomeProdotto, Money prezzoUnitario, int quantita)
        {
            Ordine = ordine;
            OrdineId = ordine.Id;
            ProdottoId = prodottoId;
            NomeProdotto = nomeProdotto;
            PrezzoUnitario = prezzoUnitario;
            Quantita = quantita;
        }
    }
}
