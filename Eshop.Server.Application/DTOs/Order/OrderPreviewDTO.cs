using System;
using System.Collections.Generic;

namespace Eshop.Server.Application.DTOs.Order
{
    /// <summary>
    /// Risultato della fase di "preview" dell'order:
    /// mostra quali righe del cart sono davvero ordinabili
    /// e quali invece vengono scartate (con il motivo).
    /// </summary>
    public class OrderPreviewDTO
    {
        /// <summary>
        /// Righe che possono essere effettivamente trasformate in order.
        /// </summary>
        public List<OrderRowDTO> ValidRows { get; set; } = new();

        /// <summary>
        /// Righe che NON possono essere ordinate (es. stock insufficiente).
        /// </summary>
        public List<UnorderableRowDTO> DiscardedRows { get; set; } = new();

        /// <summary>
        /// Total calcolato solo sulle righe valide.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Eventuale token di correlazione tra preview e conferma.
        /// Puoi anche non usarlo per ora.
        /// </summary>
        public string? Token { get; set; }
    }

    // ======================================
    //  DTO di supporto per le righe della preview
    // ======================================

    /// <summary>
    /// Riga che può essere ordinata.
    /// È uno "snapshot" del product in quel momento.
    /// </summary>
    public class OrderRowDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Price unitario corrente del product.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Quantity realmente ordinabile.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Subtotal = price * quantity.
        /// </summary>
        public decimal Subtotal => UnitPrice * Quantity;
    }

    /// <summary>
    /// Riga che NON può essere ordinata, con il motivo dello scarto.
    /// </summary>
    public class UnorderableRowDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Spiega perché questa riga non è ordinabile
        /// (es. "disponibili solo 2", "product disattivato"...).
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}
