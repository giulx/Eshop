using ApiServer.Application.DTOs.Utente;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;

namespace ApiServer.Application.Services
{
    public class UtenteService
    {
        private readonly IUtenteRepository _utenteRepository;

        public UtenteService(IUtenteRepository utenteRepository)
        {
            _utenteRepository = utenteRepository;
        }

        public async Task<UtenteReadDTO?> GetByIdAsync(int id)
        {
            var utente = await _utenteRepository.GetByIdAsync(id);
            if (utente == null) return null;

            return new UtenteReadDTO
            {
                Id = utente.Id,
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                Email = utente.Email,
                IsAdmin = utente.IsAdmin
            };
        }

        public async Task<bool> CreaUtenteAsync(UtenteCreateDTO dto, string passwordHash)
        {
            var esistente = await _utenteRepository.GetByEmailAsync(dto.Email);
            if (esistente != null) return false;

            var utente = new Utente(
                dto.Nome,
                dto.Cognome,
                dto.Email,
                passwordHash
            );

            await _utenteRepository.AddAsync(utente);
            return true;
        }

        public async Task<bool> AggiornaUtenteAsync(int id, UtenteUpdateDTO dto, string? nuovaPasswordHash = null)
        {
            var utente = await _utenteRepository.GetByIdAsync(id);
            if (utente == null) return false;

            utente.AggiornaDatiPersonali(dto.Nome, dto.Cognome);

            if (!string.IsNullOrEmpty(dto.Indirizzo))
            {
                utente.AggiornaIndirizzo(new Domain.ValueObjects.Indirizzo(
                    dto.Indirizzo, dto.Citta, dto.CAP, dto.Telefono));
            }

            if (!string.IsNullOrEmpty(nuovaPasswordHash))
            {
                utente.AggiornaPasswordHash(nuovaPasswordHash);
            }

            await _utenteRepository.UpdateAsync(utente);
            return true;
        }
    }
}
