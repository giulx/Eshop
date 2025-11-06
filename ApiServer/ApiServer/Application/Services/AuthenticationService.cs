using System.Threading.Tasks;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;
using ApiServer.Domain.ValueObjects;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// Implementazione del servizio di autenticazione.
    /// Coordina la registrazione, il login e la generazione di token JWT.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUtenteRepository _utenteRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthenticationService(
            IUtenteRepository utenteRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _utenteRepository = utenteRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string nome, string cognome, string email, string password)
        {
            var existingUser = await _utenteRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return (false, "L'indirizzo email è già registrato.");

            var passwordHash = _passwordHasher.Hash(password);
            var nuovoUtente = new Utente(nome, cognome, new Email(email), passwordHash);

            await _utenteRepository.AddAsync(nuovoUtente);
            return (true, "Registrazione completata con successo.");
        }

        public async Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password)
        {
            var utente = await _utenteRepository.GetByEmailAsync(email);
            if (utente == null)
                return (false, null, "Nessun account trovato con questa email.");

            if (!_passwordHasher.Verify(password, utente.PasswordHash))
                return (false, null, "Password errata.");

            var ruolo = utente.IsAdmin ? "Admin" : "Cliente";
            var token = _jwtService.GenerateToken(utente.Id, utente.Email.Value, ruolo);

            return (true, token, "Login effettuato con successo.");
        }
    }
}
