using AutoMapper;
using ApiServer.Domain.Models;
using ApiServer.Application.DTOs.Utente;
using ApiServer.Application.DTOs.Prodotto;
using ApiServer.Application.DTOs.Ordine;

namespace ApiServer.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // =========================
            // Utente
            // =========================
            CreateMap<Utente, UtenteReadDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Address))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.IsAdmin ? "Admin" : "Cliente"));

            CreateMap<UtenteUpdateDTO, Utente>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // email non modificabile
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.Role == "Admin"));

            // =========================
            // Prodotto
            // =========================
            CreateMap<Prodotto, ProdottoReadDTO>()
                .ForMember(dest => dest.Prezzo, opt => opt.MapFrom(src => src.Prezzo.Amount));

            CreateMap<ProdottoCreateDTO, Prodotto>()
                .ForMember(dest => dest.Prezzo, opt => opt.MapFrom(src => new Domain.ValueObjects.Money(src.Prezzo)));

            // =========================
            // Ordine
            // =========================
            CreateMap<Ordine, OrdineReadDTO>()
                .ForMember(dest => dest.ClienteEmail, opt => opt.MapFrom(src => src.Cliente.Email.Address));

            CreateMap<OrdineCreateDTO, Ordine>();

            // =========================
            // VoceOrdine
            // =========================
            CreateMap<VoceOrdine, VoceOrdineReadDTO>()
                .ForMember(dest => dest.Prezzo, opt => opt.MapFrom(src => src.Prezzo.Amount));

            CreateMap<VoceOrdineCreateDTO, VoceOrdine>()
                .ForMember(dest => dest.Prezzo, opt => opt.MapFrom(src => new Domain.ValueObjects.Money(src.Prezzo)));
        }
    }
}
