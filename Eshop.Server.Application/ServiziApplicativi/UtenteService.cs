using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Utente;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;
using Microsoft.AspNetCore.Identity;

namespace Eshop.Server.Application.ServiziApplicativi
{
    /// <summary>
    /// Servizio applicativo per la gestione degli utenti.
    /// </summary>
    public class UtenteService
    {
        private readonly IUtenteRepository _utenteRepository;
        private readonly ICarrelloRepository _carrelloRepository;
        private readonly IPasswordHasher<string> _passwordHasher;

        public UtenteService(
            IUtenteRepository utenteRepository,
            ICarrelloRepository carrelloRepository,
            IPasswordHasher<string> passwordHasher)
        {
            _utenteRepository = utenteRepository;
            _carrelloRepository = carrelloRepository;
            _passwordHasher = passwordHasher;
        }

        // =========================================================
        // READ
        // =========================================================

        /// <summary>
        /// Restituisce i dati di un utente dato il suo Id.
        /// </summary>
        public async Task<UtenteReadDTO?> GetByIdAsync(int id)
        {
            var utente = await _utenteRepository.GetByIdAsync(id);
            if (utente == null)
                return null;

            return MapToReadDto(utente);
        }

        /// <summary>
        /// Restituisce tutti gli utenti (versione semplice, senza paginazione).
        /// </summary>
        public async Task<List<UtenteReadDTO>> GetAllAsync()
        {
            var utenti = await _utenteRepository.GetAllAsync();
            return utenti.Select(MapToReadDto).ToList();
        }

        /// <summary>
        /// Restituisce gli utenti con filtro e paginazione.
        /// Pensata per admin/backoffice.
        /// </summary>
        /// <param name="search">Testo da cercare su nome, cognome o email.</param>
        /// <param name="page">Pagina (1-based).</param>
        /// <param name="pageSize">Elementi per pagina.</param>
        /// <returns>Tupla con utenti della pagina e totale complessivo.</returns>
        public async Task<(IReadOnlyList<UtenteReadDTO> Items, int TotalCount)> GetAllAsync(
            string? search,
            int page,
            int pageSize)
        {
            var utenti = await _utenteRepository.GetAllAsync();

            // filtro
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                utenti = utenti
                    .Where(u =>
                        u.Nome.ToLower().Contains(lower) ||
                        u.Cognome.ToLower().Contains(lower) ||
                        u.Email.Valore.ToLower().Contains(lower))
                    .ToList();
            }

            var total = utenti.Count();

            // paginazione in memoria
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var skip = (page - 1) * pageSize;
            var pageItems = utenti
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
        /// Crea un nuovo utente cliente. Se l'email esiste già restituisce false.
        /// </summary>
        public async Task<bool> CreaUtenteAsync(UtenteCreateDTO dto)
        {
            // 1. controllo duplicato email
            var esistente = await _utenteRepository.GetByEmailAsync(new Email(dto.Email));
            if (esistente != null)
                return false;

            // 2. hash password
            var passwordHash = _passwordHasher.HashPassword(dto.Email, dto.Password);

            // 3. creo utente di dominio (sempre non admin da API)
            var utente = new Utente(
                dto.Nome,
                dto.Cognome,
                new Email(dto.Email),
                passwordHash,
                isAdmin: false
            );

            // 4. mappo eventuale indirizzo
            if (!string.IsNullOrWhiteSpace(dto.Indirizzo) ||
                !string.IsNullOrWhiteSpace(dto.Citta) ||
                !string.IsNullOrWhiteSpace(dto.CAP) ||
                !string.IsNullOrWhiteSpace(dto.Telefono))
            {
                var indirizzoVO = new Indirizzo(
                    via: dto.Indirizzo ?? string.Empty,
                    citta: dto.Citta ?? string.Empty,
                    cap: dto.CAP ?? string.Empty,
                    telefono: dto.Telefono ?? string.Empty
                );

                utente.AggiornaIndirizzo(indirizzoVO);
            }

            // 5. salvo l'utente
            await _utenteRepository.AddAsync(utente);

            // 6. creo il carrello per il cliente
            var carrello = new Carrello(utente);
            await _carrelloRepository.AddAsync(carrello);

            return true;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        /// <summary>
        /// Aggiorna i dati di un utente (anagrafica, indirizzo, password).
        /// </summary>
        public async Task<bool> AggiornaUtenteAsync(int id, UtenteUpdateDTO dto)
        {
            var utente = await _utenteRepository.GetByIdAsync(id);
            if (utente == null)
                return false;

            // 1. anagrafica
            var nuovoNome = string.IsNullOrWhiteSpace(dto.Nome) ? utente.Nome : dto.Nome!;
            var nuovoCognome = string.IsNullOrWhiteSpace(dto.Cognome) ? utente.Cognome : dto.Cognome!;
            utente.AggiornaDatiPersonali(nuovoNome, nuovoCognome);

            // 2. indirizzo (ricomponiamo il VO con vecchi + nuovi dati)
            var arrivaQualcheDatoIndirizzo =
                !string.IsNullOrWhiteSpace(dto.Indirizzo) ||
                !string.IsNullOrWhiteSpace(dto.Citta) ||
                !string.IsNullOrWhiteSpace(dto.CAP) ||
                !string.IsNullOrWhiteSpace(dto.Telefono);

            if (arrivaQualcheDatoIndirizzo)
            {
                var attuale = utente.Indirizzo;
                var via = !string.IsNullOrWhiteSpace(dto.Indirizzo) ? dto.Indirizzo! : attuale?.Via ?? string.Empty;
                var citta = !string.IsNullOrWhiteSpace(dto.Citta) ? dto.Citta! : attuale?.Citta ?? string.Empty;
                var cap = !string.IsNullOrWhiteSpace(dto.CAP) ? dto.CAP! : attuale?.CAP ?? string.Empty;
                var telefono = !string.IsNullOrWhiteSpace(dto.Telefono) ? dto.Telefono! : attuale?.Telefono ?? string.Empty;

                var nuovoIndirizzo = new Indirizzo(via, citta, cap, telefono);
                utente.AggiornaIndirizzo(nuovoIndirizzo);
            }

            // 3. password
            if (!string.IsNullOrWhiteSpace(dto.NuovaPassword))
            {
                var nuovaHash = _passwordHasher.HashPassword(
                    utente.Email.Valore,
                    dto.NuovaPassword
                );
                utente.AggiornaPasswordHash(nuovaHash);
            }

            await _utenteRepository.UpdateAsync(utente);
            return true;
        }

        // =========================================================
        // DELETE
        // =========================================================

        /// <summary>
        /// Elimina un utente. Per sicurezza non elimina gli admin.
        /// </summary>
        public async Task<bool> EliminaUtenteAsync(int id)
        {
            var utente = await _utenteRepository.GetByIdAsync(id);
            if (utente is null)
                return false;

            // protezione: non buttiamo giù l'admin
            if (utente.IsAdmin)
                return false;

            return await _utenteRepository.DeleteAsync(id);
        }

        // =========================================================
        // PRIVATE
        // =========================================================

        /// <summary>
        /// Mapping centralizzato dominio → DTO.
        /// </summary>
        private static UtenteReadDTO MapToReadDto(Utente utente)
        {
            return new UtenteReadDTO
            {
                Id = utente.Id,
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                Email = utente.Email.Valore,
                IsAdmin = utente.IsAdmin,
                Indirizzo = utente.Indirizzo?.Via,
                Citta = utente.Indirizzo?.Citta,
                CAP = utente.Indirizzo?.CAP,
                Telefono = utente.Indirizzo?.Telefono
            };
        }
    }
}
