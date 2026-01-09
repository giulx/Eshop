using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.User
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s]+$", ErrorMessage = "Il nome può contenere solo lettere, spazi, apostrofi e trattini.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s]+$", ErrorMessage = "Il cognome può contenere solo lettere, spazi, apostrofi e trattini.")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Formato email non valido.")]
        [StringLength(254, ErrorMessage = "L'email non può superare i 254 caratteri.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve contenere almeno 8 caratteri.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La via è obbligatoria.")]
        [StringLength(200, ErrorMessage = "La via non può superare i 200 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s'\.,\-\/]+$", ErrorMessage = "La via può contenere solo lettere, numeri, spazi, apostrofi, punti, virgole, trattini e slash.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "La città è obbligatoria.")]
        [StringLength(200, ErrorMessage = "La città non può superare i 200 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}'\-\s\.]+$", ErrorMessage = "La città può contenere solo lettere, spazi, apostrofi, trattini e punti.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il CAP è obbligatorio.")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Il CAP deve essere compreso tra 3 e 10 caratteri.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Il CAP deve contenere solo cifre.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il numero civico è obbligatorio.")]
        [StringLength(20, ErrorMessage = "Il numero civico non può superare i 20 caratteri.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s\/\-]+$", ErrorMessage = "Il numero civico può contenere solo lettere, numeri, spazi, trattini e slash.")]
        public string Number { get; set; } = string.Empty;
    }
}
