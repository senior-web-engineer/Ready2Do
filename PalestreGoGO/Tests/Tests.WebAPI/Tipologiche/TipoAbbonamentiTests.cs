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
    public class TipoAbbonamentiTestsFixture
    {
        public TipologieAbbonamentiViewModel Entity;
    }

    public class TipoAbbonamentiTests : BaseTipologicheTests, IClassFixture<TipoAbbonamentiTestsFixture>
    {

        private TipoAbbonamentiTestsFixture _fixture;

        private static Random s_random = new Random();

        public TipoAbbonamentiTests(ITestOutputHelper output, TipoAbbonamentiTestsFixture fixture) : base(output)
        {
            _fixture = fixture;
        }

        private Mock<TipoAbbonamentiAPIController> SetupController(ClaimsPrincipal principal)
        {
            var loggerMock = new Mock<ILogger<TipoAbbonamentiAPIController>>();
            var dbCtx = Utils.BuildDbContext();
            var repo = new TipologieAbbonamentiRepository(dbCtx);
            var controllerMocked = new Mock<TipoAbbonamentiAPIController>(repo, loggerMock.Object);
            controllerMocked.CallBase = true;
            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
            return controllerMocked;
        }

        [Fact, TestOrder(1)]
        public void Crea_tipo_abbonamento()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _fixture.Entity = new TipologieAbbonamentiViewModel()
            {
                Costo = 1020.3M,
                DurataMesi = 4,
                MaxLivCorsi = 300,
                NumIngressi = 1000,
                Nome = "Quadrimestrale 300",
            };
            var result = controller.Object.Create(Utils.ID_CLIENTE_TEST_1, _fixture.Entity);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            _fixture.Entity.Id = id;
        }


        [Fact, TestOrder(3)]
        public void Get_tipo_abboanmento()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoAbbonamento = okResult.Value.Should().BeAssignableTo<TipologieAbbonamentiViewModel>().Subject;
            tipoAbbonamento.Id.Should().Be(_fixture.Entity.Id);
            tipoAbbonamento.Nome.Should().Be(_fixture.Entity.Nome);
            tipoAbbonamento.Costo.Should().Be(_fixture.Entity.Costo);
            tipoAbbonamento.DurataMesi.Should().Be(_fixture.Entity.DurataMesi);
            tipoAbbonamento.MaxLivCorsi.Should().Be(_fixture.Entity.MaxLivCorsi);
            tipoAbbonamento.NumIngressi.Should().Be(_fixture.Entity.NumIngressi);
        }

        [Fact, TestOrder(5)]
        public void Modify_tipo_abbonamento()
        {
            output.WriteLine("Executing Modify_tipo_lezione ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _fixture.Entity.Nome = _fixture.Entity.Nome + "-MODIFIED";
            _fixture.Entity.Costo = _fixture.Entity.Costo * 2;
            _fixture.Entity.DurataMesi = 6;
            _fixture.Entity.MaxLivCorsi = 450;
            _fixture.Entity.NumIngressi = 20;
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1, _fixture.Entity);
            result.Should().BeOfType<OkResult>();
        }

        [Fact, TestOrder(6)]
        public void Modify_tipo_abbonamento_wrongTenant()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1 + 10, _fixture.Entity);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact, TestOrder(7)]
        public void Get_tipo_abbonamento_modified()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoAbbonamento = okResult.Value.Should().BeAssignableTo<TipologieAbbonamentiViewModel>().Subject;
            tipoAbbonamento.Id.Should().Be(_fixture.Entity.Id);
            tipoAbbonamento.Nome.Should().Be(_fixture.Entity.Nome);
            tipoAbbonamento.Costo.Should().Be(_fixture.Entity.Costo);
            tipoAbbonamento.DurataMesi.Should().Be(_fixture.Entity.DurataMesi);
            tipoAbbonamento.MaxLivCorsi.Should().Be(_fixture.Entity.MaxLivCorsi);
            tipoAbbonamento.NumIngressi.Should().Be(_fixture.Entity.NumIngressi);
        }

        [Fact, TestOrder(20)]
        public void Get_tipo_abbonamento_cliente_errato()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_2, _fixture.Entity.Id.Value);
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact, TestOrder(40)]
        public void Get_all_tipi_abbonamenti()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetAll(Utils.ID_CLIENTE_TEST_1);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipiAbbonamenti = okResult.Value.Should().BeAssignableTo<IEnumerable<TipologieAbbonamentiViewModel>>().Subject;
            tipiAbbonamenti.Count().Should().BeGreaterThan(0);
            var tipoAbbonamento = tipiAbbonamenti.Should().ContainSingle(tl => tl.Id == _fixture.Entity.Id).Subject;
            tipoAbbonamento.Id.Should().Be(_fixture.Entity.Id);
            tipoAbbonamento.Nome.Should().Be(_fixture.Entity.Nome);
            tipoAbbonamento.DurataMesi.Should().Be(_fixture.Entity.DurataMesi);
            tipoAbbonamento.Costo.Should().Be(_fixture.Entity.Costo);
            tipoAbbonamento.MaxLivCorsi.Should().Be(_fixture.Entity.MaxLivCorsi);
            tipoAbbonamento.NumIngressi.Should().Be(_fixture.Entity.NumIngressi);
        }

        [Fact, TestOrder(50)]
        public void Delete_tipo_abbonamento()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Delete(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
