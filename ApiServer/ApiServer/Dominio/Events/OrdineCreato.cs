using System;
using ApiServer.Domain.Models;

namespace ApiServer.Domain.Events
{
    public class OrdineCreato
    {
        public Ordine Ordine { get; }
        public DateTime DataEvento { get; }

        public OrdineCreato(Ordine ordine)
        {
            Ordine = ordine ?? throw new ArgumentNullException(nameof(ordine));
            DataEvento = DateTime.UtcNow;
        }
    }
}

