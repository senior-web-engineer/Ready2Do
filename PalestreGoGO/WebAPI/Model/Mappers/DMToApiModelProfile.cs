using AutoMapper;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.Mappers
{
    public class DMToApiModelProfile: Profile
    {
        public DMToApiModelProfile()
        {
            CreateMap<LocationDM, LocationApiModel>()
                .ReverseMap();
            CreateMap<TipologiaLezioneDM, TipologieLezioniApiModel>()
                .ReverseMap();
            this.CreateMap<ScheduleDM, ScheduleDetailedApiModel>()
                .ReverseMap()
                .ForMember(d => d.DataCancellazione, opt => opt.Ignore())
                .ForMember(d => d.UserIdOwner, opt => opt.Ignore());            

        }
    }
}
