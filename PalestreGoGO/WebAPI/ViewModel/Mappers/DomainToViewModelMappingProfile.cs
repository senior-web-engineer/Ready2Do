using AutoMapper;
using PalestreGoGo.DataModel;
using PalestreGoGo.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<TipologieClienti, TipologieClientiViewModel>();
            CreateMap<TipologieImmagini, TipologieImmaginiViewModel>();
            CreateMap<TipologieLezioni, TipologieLezioniViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());
            CreateMap<TipologieAbbonamenti, TipologieAbbonamentiViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore())
                .ForPath(x => x.AbbonamentiUtenti, x => x.Ignore());
            CreateMap<Locations, LocationViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore())
                .ForPath(x => x.Schedules, x => x.Ignore());

            CreateMap<Clienti, ClienteViewModel>()
                .ForMember(d => d.IdCliente, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.UrlImmagineHome, opt => opt.MapFrom(src => src.ClientiImmagini.First().Url))
                .ForMember(d => d.OrarioApertura, opt => opt.MapFrom(src => src.ClientiMetadati.Where(md => md.Key.Equals(DataModel.Constants.METADATI_KEY_ORARIO_APERTURA))));

            CreateMap<Schedules, ScheduleViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());

            CreateMap<Schedules, ScheduleDetailsViewModel>();

            CreateMap<AppUser, ClienteUtenteViewModel>()
                .ForMember(d => d.Nome, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.Cognome, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email));
        }

    }
}
