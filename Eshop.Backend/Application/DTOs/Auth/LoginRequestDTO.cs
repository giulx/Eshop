using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Formato email non valido.")]
        [StringLength(254, ErrorMessage = "L'email non può superare i 254 caratteri.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "La password deve contenere almeno 8 caratteri.")]
        public string Password { get; set; } = string.Empty;
    }
}


