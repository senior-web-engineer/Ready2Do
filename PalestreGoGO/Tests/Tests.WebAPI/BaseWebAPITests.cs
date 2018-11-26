using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PalestreGoGo.WebAPI.ViewModel.Mappers;
using System;
using System.Security.Claims;
using Xunit.Abstractions;

namespace Tests.WebAPI
{
    public abstract class BaseWebApiTests : IDisposable
    {
        private static bool _automapperInitialized = false;

        protected readonly ITestOutputHelper output;

        public static void InitAutoMapper()
        {
            if (!_automapperInitialized)
            {
                _automapperInitialized = true;
                Mapper.Initialize(x =>
                {
                    x.AddProfile<DomainToViewModelMappingProfile>();
                    x.AddProfile<DMToApiModelProfile>();
                });
            }
        }

        public BaseWebApiTests(ITestOutputHelper output)
        {
            InitAutoMapper();
            this.output = output;
        }


        public virtual void Dispose()
        {
        }
    }
}
