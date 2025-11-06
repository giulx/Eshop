using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Ordine
{
    /// <summary>
    /// DTO di esito della conferma ordine.
    /// Evita i null: esplicita successo, errori e ordine creato.
    /// </summary>
    public class ConfermaOrdineResultDTO
    {
        /// <summary>
        /// True se l’ordine è stato creato con successo.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Codice di errore in caso di fallimento (es. stock_changed, payment_failed...).
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Ordine creato (solo se Success = true).
        /// </summary>
        public OrdineReadDTO? Ordine { get; set; }

        /// <summary>
        /// Righe non ordinate (se ci sono problemi di stock).
        /// </summary>
        public List<RigaNonOrdinabileDTO>? RigheNonOrdinate { get; set; }
    }
}

