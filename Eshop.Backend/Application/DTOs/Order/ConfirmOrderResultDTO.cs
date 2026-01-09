using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Order
{
    /// <summary>
    /// DTO di esito della conferma order.
    /// Evita i null: esplicita successo, errori e order creato.
    /// </summary>
    public class ConfirmOrderResultDTO
    {
        /// <summary>
        /// True se l’order è status creato con successo.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Codice di errore in caso di fallimento (es. stock_changed, payment_failed...).
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Order creato (solo se Success = true).
        /// </summary>
        public OrderReadDTO? Order { get; set; }

        /// <summary>
        /// Righe non ordinate (se ci sono problemi di stock).
        /// </summary>
        public List<UnorderableRowDTO>? UnorderedRows { get; set; }
    }
}

