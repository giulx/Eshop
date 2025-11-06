using ApiServer.Domain.Models;
using ApiServer.Domain.ValueObjects;
using System;

namespace ApiServer.Domain.Services
{
    public class GestioneOrdineService
    {
        /// <summary>
        /// Aggiunge un prodotto a un ordine esistente, aggiornando la quantità se già presente.
        /// </summary>
        public void AggiungiProdotto(Ordine ordine, Prodotto prodotto, Quantita quantita)
        {
            if (ordine == null) throw new ArgumentNullException(nameof(ordine));
            if (prodotto == null) throw new ArgumentNullException(nameof(prodotto));
            if (quantita == null) throw new ArgumentNullException(nameof(quantita));

            var voceEsistente = ordine.Voci.Find(v => v.Prodotto.Id == prodotto.Id);
            if (voceEsistente != null)
            {
                // Non vogliamo cambiare direttamente VO Quantita? Possiamo creare metodo Ordine per questo
                throw new InvalidOperationException("Prodotto già presente nell'ordine. Aggiornare la quantità tramite Ordine.");
            }
            else
            {
                ordine.AggiungiVoce(prodotto, quantita);
            }
        }
    }
}
