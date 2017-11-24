using AutoMapper;
using PalestreGoGo.DataModel;
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
            CreateMap<TipologieAbbonamenti, TipologieAbbonamentiViewModel>();
            CreateMap<TipologieClienti, TipologieClientiViewModel>();
            CreateMap<TipologieImmagini, TipologieImmaginiViewModel>();
            CreateMap<TipologieLezioni, TipologieLezioniViewModel>()
                .ReverseMap()
                .ForPath(x => x.IdClienteNavigation, x => x.Ignore());
        }

    }
}
