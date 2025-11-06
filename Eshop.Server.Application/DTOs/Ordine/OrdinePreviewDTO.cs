using System;
using System.Collections.Generic;

namespace Eshop.Server.Applicazione.DTOs.Ordine
{
    /// <summary>
    /// Risultato della fase di "preview" dell'ordine:
    /// mostra quali righe del carrello sono davvero ordinabili
    /// e quali invece vengono scartate (con il motivo).
    /// </summary>
    public class OrdinePreviewDTO
    {
        /// <summary>
        /// Righe che possono essere effettivamente trasformate in ordine.
        /// </summary>
        public List<RigaOrdineDTO> RigheValide { get; set; } = new();

        /// <summary>
        /// Righe che NON possono essere ordinate (es. stock insufficiente).
        /// </summary>
        public List<RigaNonOrdinabileDTO> RigheScartate { get; set; } = new();

        /// <summary>
        /// Totale calcolato solo sulle righe valide.
        /// </summary>
        public decimal Totale { get; set; }

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
    /// È uno "snapshot" del prodotto in quel momento.
    /// </summary>
    public class RigaOrdineDTO
    {
        public int ProdottoId { get; set; }

        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Prezzo unitario corrente del prodotto.
        /// </summary>
        public decimal PrezzoUnitario { get; set; }

        /// <summary>
        /// Quantità realmente ordinabile.
        /// </summary>
        public int Quantita { get; set; }

        /// <summary>
        /// Subtotale = prezzo * quantità.
        /// </summary>
        public decimal Subtotale => PrezzoUnitario * Quantita;
    }

    /// <summary>
    /// Riga che NON può essere ordinata, con il motivo dello scarto.
    /// </summary>
    public class RigaNonOrdinabileDTO
    {
        public int ProdottoId { get; set; }

        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Spiega perché questa riga non è ordinabile
        /// (es. "disponibili solo 2", "prodotto disattivato"...).
        /// </summary>
        public string Motivo { get; set; } = string.Empty;
    }
}
