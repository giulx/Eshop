using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Auth;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Eshop.Server.Application.ApplicationServices
{
    /// <summary>
    /// Servizio applicativo che si occupa di autenticazione (login).
    /// </summary>
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<string> passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResultDTO> LoginAsync(LoginRequestDTO dto)
        {
            // 1. prendo l'user per email
            var user = await _userRepository.GetByEmailAsync(new Email(dto.Email));
            if (user == null)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorCode = "user_not_found",
                    Message = "Nessun account trovato con questa email."
                };
            }

            // (esempio opzionale: se hai flag tipo IsActive / EmailConfirmed)
            // if (!user.EmailConfirmed)
            // {
            //     return new LoginResultDTO
            //     {
            //         Success   = false,
            //         ErrorCode = "email_not_confirmed",
            //         Message   = "Email non confermata. Controlla la tua casella di posta."
            //     };
            // }

            // 2. verifico la password
            var verifica = _passwordHasher.VerifyHashedPassword(
                dto.Email,
                user.PasswordHash,
                dto.Password
            );

            if (verifica == PasswordVerificationResult.Failed)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorCode = "wrong_password",
                    Message = "Password errata."
                };
            }

            // 3. password ok → genero JWT
            var token = _jwtService.GenerateToken(user);

            return new LoginResultDTO
            {
                Success = true,
                ErrorCode = null,
                Message = "Login effettuato.",
                UserId = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                IsAdmin = user.IsAdmin,
                Token = token
            };
        }
    }
}
