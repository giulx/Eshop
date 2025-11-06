using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Auth;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.OggettiValore;
using Microsoft.AspNetCore.Identity;

namespace Eshop.Server.Application.ServiziApplicativi
{
    /// <summary>
    /// Servizio applicativo che si occupa di autenticazione (login).
    /// </summary>
    public class AuthService
    {
        private readonly IUtenteRepository _utenteRepository;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUtenteRepository utenteRepository,
            IPasswordHasher<string> passwordHasher,
            IJwtService jwtService)
        {
            _utenteRepository = utenteRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResultDTO> LoginAsync(LoginRequestDTO dto)
        {
            // 1. prendo l'utente per email
            var utente = await _utenteRepository.GetByEmailAsync(new Email(dto.Email));
            if (utente == null)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    Message = "Credenziali non valide."
                };
            }

            // 2. verifico la password
            var verifica = _passwordHasher.VerifyHashedPassword(
                dto.Email,
                utente.PasswordHash,
                dto.Password
            );

            if (verifica == PasswordVerificationResult.Failed)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    Message = "Credenziali non valide."
                };
            }

            // 3. password ok → genero JWT
            var token = _jwtService.GeneraToken(utente);

            return new LoginResultDTO
            {
                Success = true,
                Message = "Login effettuato.",
                UtenteId = utente.Id,
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                IsAdmin = utente.IsAdmin,
                Token = token
            };
        }
    }
}

