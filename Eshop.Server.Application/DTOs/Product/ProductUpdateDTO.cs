using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.Product
{
    public class ProductUpdateDTO
    {
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri.")]
        [RegularExpression(
            @"^[\p{L}\p{M}0-9\s'\-.,]+$",
            ErrorMessage = "Il nome può contenere solo lettere, numeri, spazi, apostrofi, trattini, punti e virgole.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "La descrizione non può superare i 500 caratteri.")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0,01", "999999,99",
            ErrorMessage = "Il prezzo deve essere compreso tra 0,01 e 999.999,99.")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "La valuta deve avere esattamente 3 caratteri (es. EUR).")]
        [RegularExpression(
            @"^[A-Z]{3}$",
            ErrorMessage = "La valuta deve essere un codice ISO a 3 lettere maiuscole (es. EUR, USD).")]
        public string? Currency { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La quantità deve essere maggiore o uguale a zero.")]
        public int? AvailableQuantity { get; set; }
    }
}
