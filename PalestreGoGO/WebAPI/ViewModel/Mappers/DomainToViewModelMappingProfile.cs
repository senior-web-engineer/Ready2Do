﻿using AutoMapper;
using PalestreGoGo.DataModel;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.Mappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        private const int TIPOIMMAGINE_LOGO = 1;
        private const int TIPOIMMAGINE_SFONDO = 2;
        private const int TIPOIMMAGINE_GALLERY = 3;

        public DomainToViewModelMappingProfile()
        {
            CreateMap<TipologiaCliente, TipologiaClienteViewModel>();
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
                .ForPath(d => d.Indrizzo.Citta, opt => opt.MapFrom(src => src.Citta))
                .ForPath(d => d.Indrizzo.Citta, opt => opt.MapFrom(src => src.Citta))
                .ForPath(d => d.Indrizzo.Coordinate.Latitudine, opt => opt.MapFrom(src => src.Latitudine))
                .ForPath(d => d.Indrizzo.Coordinate.Longitudine, opt => opt.MapFrom(src => src.Longitudine))
                .ForPath(d => d.Indrizzo.PostalCode, opt => opt.MapFrom(src => src.ZipOrPostalCode))
                .ForPath(d => d.Indrizzo.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(d => d.ImmagineHome, opt =>
                                    opt.MapFrom(src => Mapper.Map<ClientiImmagini, ImmagineViewModel>(
                                    src.ClientiImmagini.Where(i => i.IdTipoImmagine.Equals(TIPOIMMAGINE_SFONDO)).FirstOrDefault())))
                .ForMember(d => d.OrarioApertura, opt => opt.MapFrom(src => src.ClientiMetadati.Count > 0 ? src.ClientiMetadati.Where(md => md.Key.Equals(DataModel.Constants.METADATI_KEY_ORARIO_APERTURA)) : null))
                .ForMember(d => d.Immagini, opt => opt.MapFrom(src => Mapper.Map<ICollection<ClientiImmagini>, ICollection<ImmagineViewModel>>(src.ClientiImmagini)));


            CreateMap<Schedules, ScheduleViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());

            CreateMap<Schedules, ScheduleDetailsViewModel>();

            CreateMap<AppUser, ClienteUtenteViewModel>()
                .ForMember(d => d.Nome, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.Cognome, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<AbbonamentiUtenti, AbbonamentoViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdTipoAbbonamentoNavigation, x => x.Ignore())
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());

        }

    }
}
