using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Tests.WebAPI.Tipologiche
{
    public class TipoLezioniTestsFixture
    {
        public TipologieLezioniViewModel TipoLezione;
    }

    public class TipoLezioniTests : BaseTipologicheTests, IClassFixture<TipoLezioniTestsFixture>
    {

        private TipoLezioniTestsFixture _tipoLezioneFixture;

        private static Random s_random = new Random();

        public TipoLezioniTests(ITestOutputHelper output, TipoLezioniTestsFixture tipoLezioneFixture) : base(output)
        {
            _tipoLezioneFixture = tipoLezioneFixture;
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

        [Fact, TestOrder(1)]
        public void Crea_tipo_lezione()
        {
            output.WriteLine("Executing Crea_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _tipoLezioneFixture.TipoLezione = new TipologieLezioniViewModel()
            {
                Nome = "Samba a gogo!",
                Descrizione = "Lezioni di samba per tutti",
                Durata = s_random.Next(30, 60),
                Livello = (short)s_random.Next(10, 600),
                MaxPartecipanti = s_random.Next(5, 20),
                LimiteCancellazioneMinuti = (short)s_random.Next(60, 120)
            };
            var result = controller.Object.Create(Utils.ID_CLIENTE_TEST_1, _tipoLezioneFixture.TipoLezione);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            _tipoLezioneFixture.TipoLezione.Id = id;
        }


        [Fact, TestOrder(3)]
        public void Get_tipo_lezione()
        {
            output.WriteLine("Executing Get_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _tipoLezioneFixture.TipoLezione.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoLezione = okResult.Value.Should().BeAssignableTo<TipologieLezioniViewModel>().Subject;
            tipoLezione.Id.Should().Be(_tipoLezioneFixture.TipoLezione.Id);
            tipoLezione.Nome.Should().Be(_tipoLezioneFixture.TipoLezione.Nome);
            tipoLezione.Descrizione.Should().Be(_tipoLezioneFixture.TipoLezione.Descrizione);
            tipoLezione.Durata.Should().Be(_tipoLezioneFixture.TipoLezione.Durata);
            tipoLezione.MaxPartecipanti.Should().Be(_tipoLezioneFixture.TipoLezione.MaxPartecipanti);
            tipoLezione.LimiteCancellazioneMinuti.Should().Be(_tipoLezioneFixture.TipoLezione.LimiteCancellazioneMinuti);
            tipoLezione.Livello.Should().Be(_tipoLezioneFixture.TipoLezione.Livello);
        }

        [Fact, TestOrder(5)]
        public void Modify_tipo_lezione()
        {
            output.WriteLine("Executing Modify_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _tipoLezioneFixture.TipoLezione.Nome = _tipoLezioneFixture.TipoLezione.Nome + "-MODIFIED";
            _tipoLezioneFixture.TipoLezione.Descrizione = _tipoLezioneFixture.TipoLezione.Descrizione + "-MODIFIED";
            _tipoLezioneFixture.TipoLezione.Durata = 90;
            _tipoLezioneFixture.TipoLezione.LimiteCancellazioneMinuti = 600;
            _tipoLezioneFixture.TipoLezione.Livello = 200;
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1, _tipoLezioneFixture.TipoLezione);
            var okResult = result.Should().BeOfType<OkResult>().Subject;
        }

        [Fact, TestOrder(6)]
        public void Modify_tipo_lezione_wrongTenant()
        {
            output.WriteLine("Executing Modify_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1 + 10, _tipoLezioneFixture.TipoLezione);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact, TestOrder(7)]
        public void Get_tipo_lezione_modified()
        {
            output.WriteLine("Executing Get_tipo_lezione_modified ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _tipoLezioneFixture.TipoLezione.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoLezione = okResult.Value.Should().BeAssignableTo<TipologieLezioniViewModel>().Subject;
            tipoLezione.Id.Should().Be(_tipoLezioneFixture.TipoLezione.Id);
            tipoLezione.Nome.Should().Be(_tipoLezioneFixture.TipoLezione.Nome);
            tipoLezione.Descrizione.Should().Be(_tipoLezioneFixture.TipoLezione.Descrizione);
            tipoLezione.Durata.Should().Be(_tipoLezioneFixture.TipoLezione.Durata);
            tipoLezione.MaxPartecipanti.Should().Be(_tipoLezioneFixture.TipoLezione.MaxPartecipanti);
            tipoLezione.LimiteCancellazioneMinuti.Should().Be(_tipoLezioneFixture.TipoLezione.LimiteCancellazioneMinuti);
            tipoLezione.Livello.Should().Be(_tipoLezioneFixture.TipoLezione.Livello);
        }

        [Fact, TestOrder(20)]
        public void Get_tipo_lezione_cliente_errato()
        {
            output.WriteLine("Executing Get_tipo_lezione_cliente_errato ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_2, Utils.ID_CLIENTE_TEST_1_TIPO_LEZIONE1);
            var okResult = result.Should().BeOfType<BadRequestResult>().Subject;
        }


        [Fact, TestOrder(40)]
        public void Get_all_tipi_lezioni()
        {
            output.WriteLine("Get_all_tipi_lezioni ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetAll(Utils.ID_CLIENTE_TEST_1);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipiLezioni = okResult.Value.Should().BeAssignableTo<IEnumerable<TipologieLezioniViewModel>>().Subject;
            tipiLezioni.Count().Should().BeGreaterThan(1);
            var tipoLezione = tipiLezioni.Should().ContainSingle(tl => tl.Id == _tipoLezioneFixture.TipoLezione.Id).Subject;
            tipoLezione.Id.Should().Be(_tipoLezioneFixture.TipoLezione.Id);
            tipoLezione.Nome.Should().Be(_tipoLezioneFixture.TipoLezione.Nome);
            tipoLezione.Descrizione.Should().Be(_tipoLezioneFixture.TipoLezione.Descrizione);
            tipoLezione.Durata.Should().Be(_tipoLezioneFixture.TipoLezione.Durata);
            tipoLezione.MaxPartecipanti.Should().Be(_tipoLezioneFixture.TipoLezione.MaxPartecipanti);
            tipoLezione.LimiteCancellazioneMinuti.Should().Be(_tipoLezioneFixture.TipoLezione.LimiteCancellazioneMinuti);
            tipoLezione.Livello.Should().Be(_tipoLezioneFixture.TipoLezione.Livello);
        }

        [Fact, TestOrder(50)]
        public void Delete_tipo_lezione()
        {
            output.WriteLine("Executing Delete_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Delete(Utils.ID_CLIENTE_TEST_1, _tipoLezioneFixture.TipoLezione.Id);
            var okResult = result.Should().BeOfType<NoContentResult>().Subject;
        }
    }
}
