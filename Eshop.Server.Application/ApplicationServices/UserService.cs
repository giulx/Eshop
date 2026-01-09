using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.User;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Eshop.Server.Application.ApplicationServices
{
    /// <summary>
    /// Servizio applicativo per la gestione degli users.
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IPasswordHasher<string> _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            ICartRepository cartRepository,
            IPasswordHasher<string> passwordHasher)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _passwordHasher = passwordHasher;
        }

        // =========================================================
        // READ
        // =========================================================

        /// <summary>
        /// Restituisce i dati di un user dato il suo Id.
        /// </summary>
        public async Task<UserReadDTO?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return MapToReadDto(user);
        }

        /// <summary>
        /// Restituisce tutti gli users (versione semplice, senza paginazione).
        /// </summary>
        public async Task<List<UserReadDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToReadDto).ToList();
        }

        /// <summary>
        /// Restituisce gli users con filtro e paginazione.
        /// Pensata per admin/backoffice.
        /// </summary>
        /// <param name="search">Testo da cercare su name, surname o email.</param>
        /// <param name="page">Pagina (1-based).</param>
        /// <param name="pageSize">Elementi per pagina.</param>
        /// <returns>Tupla con users della pagina e totale complessivo.</returns>
        public async Task<(IReadOnlyList<UserReadDTO> Items, int TotalCount)> GetAllAsync(
            string? search,
            int page,
            int pageSize)
        {
            var users = await _userRepository.GetAllAsync();

            // filtro
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                users = users
                    .Where(u =>
                        u.Name.ToLower().Contains(lower) ||
                        u.Surname.ToLower().Contains(lower) ||
                        u.Email.Value.ToLower().Contains(lower))
                    .ToList();
            }

            var total = users.Count();

            // paginazione in memoria
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var skip = (page - 1) * pageSize;
            var pageItems = users
                .Skip(skip)
                .Take(pageSize)
                .Select(MapToReadDto)
                .ToList();

            return (pageItems, total);
        }

        // =========================================================
        // CREATE
        // =========================================================

        /// <summary>
        /// Crea un new user customer. Se l'email esiste già restituisce false.
        /// </summary>
        public async Task<bool> CreateUserAsync(UserCreateDTO dto)
        {
            // 1. controllo duplicato email
            var existing = await _userRepository.GetByEmailAsync(new Email(dto.Email));
            if (existing != null)
                return false;

            // 2. hash password
            var passwordHash = _passwordHasher.HashPassword(dto.Email, dto.Password);

            // 3. creo user di dominio (sempre non admin da API)
            var user = new User(
                dto.Name,
                dto.Surname,
                new Email(dto.Email),
                passwordHash,
                isAdmin: false
            );

            // 4. mappo eventuale address
            if (!string.IsNullOrWhiteSpace(dto.Street) ||
                !string.IsNullOrWhiteSpace(dto.City) ||
                !string.IsNullOrWhiteSpace(dto.PostalCode) ||
                !string.IsNullOrWhiteSpace(dto.Number))
            {
                var addressVO = new Address(
                    street: dto.Street ?? string.Empty,
                    city: dto.City ?? string.Empty,
                    postalCode: dto.PostalCode ?? string.Empty,
                    number: dto.Number ?? string.Empty
                );

                user.UpdateAddress(addressVO);
            }

            // 5. salvo l'user
            await _userRepository.AddAsync(user);

            // 6. creo il cart per il customer
            var cart = new Cart(user);
            await _cartRepository.AddAsync(cart);

            return true;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        /// <summary>
        /// Aggiorna i dati di un user (anagrafica, address, password).
        /// </summary>
        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // 1. anagrafica
            var newName = string.IsNullOrWhiteSpace(dto.Name) ? user.Name : dto.Name!;
            var newSurname = string.IsNullOrWhiteSpace(dto.Surname) ? user.Surname : dto.Surname!;
            user.UpdatePersonalData(newName, newSurname);

            // 2. address (ricomponiamo il VO con vecchi + nuovi dati)
            var hasAnyAddressData =
                !string.IsNullOrWhiteSpace(dto.Street) ||
                !string.IsNullOrWhiteSpace(dto.City) ||
                !string.IsNullOrWhiteSpace(dto.PostalCode) ||
                !string.IsNullOrWhiteSpace(dto.Number);

            if (hasAnyAddressData)
            {
                var current = user.Address;
                var street = !string.IsNullOrWhiteSpace(dto.Street) ? dto.Street! : current?.Street ?? string.Empty;
                var city = !string.IsNullOrWhiteSpace(dto.City) ? dto.City! : current?.City ?? string.Empty;
                var postalCode = !string.IsNullOrWhiteSpace(dto.PostalCode) ? dto.PostalCode! : current?.PostalCode ?? string.Empty;
                var number = !string.IsNullOrWhiteSpace(dto.Number) ? dto.Number! : current?.Number ?? string.Empty;

                var newAddress = new Address(street, city, postalCode, number);
                user.UpdateAddress(newAddress);
            }

            // 3. password
            if (!string.IsNullOrWhiteSpace(dto.NuovaPassword))
            {
                var newHash = _passwordHasher.HashPassword(
                    user.Email.Value,
                    dto.NuovaPassword
                );
                user.UpdatePasswordHash(newHash);
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }

        // =========================================================
        // DELETE
        // =========================================================

        /// <summary>
        /// Elimina un user. Per sicurezza non elimina gli admin.
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
                return false;

            // ✅ protezione: gli admin non si eliminano
            if (user.IsAdmin)
                return false;

            return await _userRepository.DeleteAsync(id);
        }

        // =========================================================
        // PRIVATE
        // =========================================================

        /// <summary>
        /// Mapping centralizzato dominio → DTO.
        /// </summary>
        private static UserReadDTO MapToReadDto(User user)
        {
            return new UserReadDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email.Value,
                IsAdmin = user.IsAdmin,
                Street = user.Address?.Street,
                City = user.Address?.City,
                PostalCode = user.Address?.PostalCode,
                Number = user.Address?.Number
            };
        }
    }
}
