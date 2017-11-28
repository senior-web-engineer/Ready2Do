using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.DataAccess;
using Moq;
using Microsoft.Extensions.Logging;
using PalestreGoGo.WebAPI.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AutoMapper;
using PalestreGoGo.WebAPI.ViewModel.Mappers;

namespace Tests.WebAPI.Tipologiche
{
    [TestClass]
    public class TestsTipologiche : BaseTipologicheTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitAutoMapper();
        }
        

        private Mock<TipoLezioniController> SetupController(ClaimsPrincipal principal)
        {
            var loggerMock = new Mock<ILogger<TipoLezioniController>>();
            var dbCtx = Utils.BuildDbContext();
            var repo = new TipologieLezioniRepository(dbCtx);
            var controllerMocked = new Mock<TipoLezioniController>(repo, loggerMock.Object);
            controllerMocked.CallBase = true;
            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
            return controllerMocked;
        }

        [TestMethod]
        public void Get_tipo_lezione()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, Utils.ID_CLIENTE_TEST_1_TIPO_LEZIONE1);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoLezione = okResult.Value.Should().BeAssignableTo<TipologieLezioniViewModel>().Subject;
            tipoLezione.Id.Should().Be(1);
            tipoLezione.Nome.Should().Be("Yoga");
            tipoLezione.Descrizione.Should().Be("Yoga per tutti");
            tipoLezione.Durata.Should().Be(50);
            tipoLezione.MaxPartecipanti.Should().Be(10);
            tipoLezione.LimiteCancellazioneMinuti.Should().Be(120);
            tipoLezione.Livello.Should().Be(150);
        }

        [TestMethod]
        public void Get_tipo_lezione_cliente_errato()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_2, Utils.ID_CLIENTE_TEST_1_TIPO_LEZIONE1);
            var okResult = result.Should().BeOfType<BadRequestResult>().Subject;
        }


        [TestMethod]
        public void Crea_tipo_lezione()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var model = new TipologieLezioniViewModel()
            {
                Nome = "Samba a gogo!",
                Descrizione = "Lezioni di samba per tutti",
                Durata = 50,
                Livello = 100,
                MaxPartecipanti = 10,
                LimiteCancellazioneMinuti = 120,
            };
            var result = controller.Object.Create(Utils.ID_CLIENTE_TEST_1, model);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);           
        }



    }
}
