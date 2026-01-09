using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.User
{
    public class UserUpdateDTO
    {
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s]+$", ErrorMessage = "Il nome può contenere solo lettere, spazi, apostrofi e trattini.")]
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s]+$", ErrorMessage = "Il cognome può contenere solo lettere, spazi, apostrofi e trattini.")]
        public string? Surname { get; set; }

        [StringLength(200, ErrorMessage = "La via non può superare i 200 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s'\.,\-\/]+$", ErrorMessage = "La via può contenere solo lettere, numeri, spazi, apostrofi, virgole, trattini, punti e slash.")]
        public string? Street { get; set; }

        [StringLength(200, ErrorMessage = "La città non può superare i 200 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s\.]+$", ErrorMessage = "La città può contenere solo lettere, spazi, apostrofi, trattini e punti.")]
        public string? City { get; set; }

        [StringLength(10, MinimumLength = 3, ErrorMessage = "Il CAP deve essere compreso tra 3 e 10 caratteri.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Il CAP deve contenere solo cifre.")]
        public string? PostalCode { get; set; }

        [StringLength(20, ErrorMessage = "Il numero civico non può superare i 20 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s\/\-]+$", ErrorMessage = "Il numero civico può contenere solo lettere, numeri, spazi, trattini e slash.")]
        public string? Number { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve contenere almeno 8 caratteri.")]
        public string? NuovaPassword { get; set; }
    }
}
