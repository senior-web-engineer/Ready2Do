//using AutoMapper;
//using PalestreGoGo.DataModel;
//using PalestreGoGo.WebAPIModel;
//using ready2do.model.common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PalestreGoGo.WebAPI.ViewModel.Mappers
//{
//    public class DomainToViewModelMappingProfile : Profile
//    {
//        public DomainToViewModelMappingProfile()
//        {
//            CreateMap<TipologiaCliente, TipologiaClienteViewModel>();
//            CreateMap<TipologieImmagini, TipologieImmaginiViewModel>();
//            CreateMap<TipologieLezioni, TipologieLezioniApiModel>()
//                .ReverseMap()
//                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());
//            CreateMap<TipologieAbbonamenti, TipologieAbbonamentiViewModel>()
//                .ReverseMap()
//                .ForPath(x => x.IdClienteNavigation, x => x.Ignore())
//                .ForPath(x => x.AbbonamentiUtenti, x => x.Ignore());
//            CreateMap<Locations, LocationApiModel>()
//                .ReverseMap()
//                .ForPath(x => x.IdClienteNavigation, x => x.Ignore())
//                .ForPath(x => x.Schedules, x => x.Ignore());

//            CreateMap<Clienti, ClienteViewModel>()
//                .ForMember(d => d.IdCliente, opt => opt.MapFrom(src => src.Id))
//                .ForPath(d => d.Indirizzo.Citta, opt => opt.MapFrom(src => src.Citta))
//                .ForPath(d => d.Indirizzo.Coordinate.Latitudine, opt => opt.MapFrom(src => src.Latitudine))
//                .ForPath(d => d.Indirizzo.Coordinate.Longitudine, opt => opt.MapFrom(src => src.Longitudine))
//                .ForPath(d => d.Indirizzo.PostalCode, opt => opt.MapFrom(src => src.ZipOrPostalCode))
//                .ForPath(d => d.Indirizzo.Country, opt => opt.MapFrom(src => src.Country))
//                .ForMember(d => d.ImmagineHome, opt =>
//                                    opt.MapFrom(src => Mapper.Map<ClientiImmagini, ImmagineViewModel>(
//                                    src.ClientiImmagini.Where(i => i.IdTipoImmagine.Equals(Constants.TIPOIMMAGINE_SFONDO)).FirstOrDefault())))
//                .ForMember(d => d.OrarioApertura, opt => opt.MapFrom(src => src.ClientiMetadati.Count > 0 ? src.ClientiMetadati.Where(md => md.Key.Equals(DataModel.Constants.METADATI_KEY_ORARIO_APERTURA)) : null))
//                .ForMember(d => d.OrarioApertura, opt => opt.Ignore()); //Skip mapping orario apertura, la facciamo a mano


//            CreateMap<Clienti, ClienteWithImagesViewModel>()
//                .ForMember(d => d.IdCliente, opt => opt.MapFrom(src => src.Id))
//                .ForPath(d => d.Indirizzo.Citta, opt => opt.MapFrom(src => src.Citta))
//                .ForPath(d => d.Indirizzo.Coordinate.Latitudine, opt => opt.MapFrom(src => src.Latitudine))
//                .ForPath(d => d.Indirizzo.Coordinate.Longitudine, opt => opt.MapFrom(src => src.Longitudine))
//                .ForPath(d => d.Indirizzo.PostalCode, opt => opt.MapFrom(src => src.ZipOrPostalCode))
//                .ForPath(d => d.Indirizzo.Country, opt => opt.MapFrom(src => src.Country))
//                .ForPath(d => d.Indirizzo.Indirizzo, opt => opt.MapFrom(src => src.Indirizzo))
//                .ForMember(d => d.ImmagineHome, opt =>
//                                    opt.MapFrom(src => Mapper.Map<ClientiImmagini, ImmagineViewModel>(
//                                    src.ClientiImmagini.Where(i => i.IdTipoImmagine.Equals(Constants.TIPOIMMAGINE_SFONDO)).FirstOrDefault())))
//                .ForMember(d => d.OrarioApertura, opt => opt.MapFrom(src => src.ClientiMetadati.Count > 0 ? src.ClientiMetadati.Where(md => md.Key.Equals(DataModel.Constants.METADATI_KEY_ORARIO_APERTURA)) : null))
//                .ForMember(d => d.Immagini, opt => 
//                                    opt.MapFrom(src => Mapper.Map<ICollection<ClientiImmagini>, ICollection<ImmagineViewModel>>(
//                                            src.ClientiImmagini.Where(i => !i.IdTipoImmagine.Equals(Constants.TIPOIMMAGINE_SFONDO)).ToList())))
//                .ForMember(d => d.OrarioApertura, opt => opt.Ignore()); //Skip mapping orario apertura, la facciamo a mano

//            CreateMap<Schedules, ScheduleApiModel>()
//                .ReverseMap()
//                .ForPath(x => x.Cliente, x => x.Ignore());

//            CreateMap<Schedules, ScheduleDetailedApiModel>()
//                .ReverseMap()
//                .ForPath(x => x.PostiResidui, opt => opt.MapFrom(src => src.PostiDisponibili)); // PostiResidui == PostiDisponibili
//                                                                                                //                .ForPath(x => x.TipologiaLezione, opt => opt.MapFrom(src => src.TipologiaLezione));

//            //CreateMap<AppUser, ClienteUtenteApiModel>()
//            //    .ForMember(d => d.Nome, opt => opt.MapFrom(src => src.FirstName))
//            //    .ForMember(d => d.Cognome, opt => opt.MapFrom(src => src.LastName))
//            //    .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
//            //    .ForMember(d => d.IdUtente, opt => opt.MapFrom(src => src.Id));

//            CreateMap<UtenteClienteAbbonamentoDM, AbbonamentoViewModel>()
//                .ReverseMap();
               

//            //CreateMap<DELME_AbbonamentiUtenti, AbbonamentoUtenteApiModel>()
//            //    .ReverseMap()
//            //    .ForMember(d => d.IdTipoAbbonamento, opt => opt.MapFrom(src => src.IdTipoAbbonamento));
//                //.ForMember(d=>d.UserId, opt=>opt.MapFrom(src=>src.UserId));


//            //CreateMap<ClienteUtenteConAbbonamento, ClienteUtenteWithAbbonamentoApiModel>();

//            //CreateMap<UserReferenceDM, UserReferenceApiModel>().ReverseMap();
//            CreateMap<NotificaDM, NotificaApiModel>().ReverseMap();
//            CreateMap<NotificaConTipoDM, NotificaConTipoApiModel>().ReverseMap();
//            CreateMap<UtenteClienteDM, ClienteUtenteApiModel>()
//                .ForMember(d=>d.IdUtente, opt => opt.MapFrom(src=>src.UserId))
//                .ReverseMap()
//                .ForMember(d => d.Stato, opt => opt.MapFrom(src => src.Stato))
//                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.IdUtente));
//            CreateMap<UtenteClienteCertificatoDM, ClienteUtenteCertificatoApiModel>()
//                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId))
//                .ReverseMap()
//                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId));


//        }

//    }
//}
