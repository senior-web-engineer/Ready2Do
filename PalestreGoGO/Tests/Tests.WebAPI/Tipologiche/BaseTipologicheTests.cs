using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PalestreGoGo.WebAPI.ViewModel.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.WebAPI.Tipologiche
{
    public abstract class BaseTipologicheTests
    {

        protected static void InitAutoMapper()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
            });
        }
    }
}
