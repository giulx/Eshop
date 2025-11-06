using AutoMapper;
using ApiServer.Models;
using ApiServer.DTOs;

namespace ApiServer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Prodotti
            CreateMap<Prodotto, ProdottoReadDto>();
            CreateMap<ProdottoCreateDto, Prodotto>();
            CreateMap<ProdottoUpdateDto, Prodotto>();

            // Ordini
            CreateMap<Ordine, OrdineReadDto>();
            CreateMap<OrdineCreateDto, Ordine>();
            CreateMap<OrdineUpdateDto, Ordine>();

            // Voci ordine
            CreateMap<VoceOrdine, VoceOrdineDTO>();
            CreateMap<VoceOrdineCreateDTO, VoceOrdine>();

            // Cliente / Utente
            CreateMap<Cliente, ClienteReadDto>();
            CreateMap<ClienteCreateDto, Cliente>();
            CreateMap<ClienteUpdateDto, Cliente>();
        }
    }
}
