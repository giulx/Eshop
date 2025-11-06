using System;
using Eshop.Server.Domain.Modelli;

namespace Eshop.Server.Domain.Eventi
{
    /// <summary>
    /// Evento di dominio che rappresenta la creazione di un nuovo ordine.
    /// Può essere utilizzato per attivare logiche di notifica, fatturazione o aggiornamento stock.
    /// </summary>
    public sealed class OrdineCreato
    {
        public Ordine Ordine { get; }
        public DateTime DataEvento { get; }

        public OrdineCreato(Ordine ordine)
        {
            Ordine = ordine ?? throw new ArgumentNullException(nameof(ordine));
            DataEvento = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"[{DataEvento:yyyy-MM-dd HH:mm:ss}] Creato ordine #{Ordine.Id} per {Ordine.Cliente.Email}";
    }
}
