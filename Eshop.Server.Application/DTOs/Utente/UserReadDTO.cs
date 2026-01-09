namespace Eshop.Server.Application.DTOs.User
{
    /// <summary>
    /// DTO di lettura per i dati dell'user.
    /// </summary>
    public class UserReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // dati “non sensibili” che puoi mostrare
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Number { get; set; }

        // se vuoi mostrarlo all’admin:
        public bool IsAdmin { get; set; }
    }
}
