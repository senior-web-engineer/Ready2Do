using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tests.Utils;
using WebAPI.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.WebAPI
{
    /*
    public class AbbonamentiTests : BaseWebApiTests
    {
        private static AbbonamentoUtenteApiModel s_abbonamento;
        private static AbbonamentoUtenteApiModel s_abbonamento2;

        public AbbonamentiTests(ITestOutputHelper output) : base(output)
        {

        }

        private Mock<AbbonamentiController> SetupController(ClaimsPrincipal principal)
        {
            var loggerMock = new Mock<ILogger<AbbonamentiController>>();
            var loggerRepoMock = new Mock<ILogger<AbbonamentiRepository>>();
            var dbCtx = Utils.BuildDbContext();
            var repo = new AbbonamentiRepository(dbCtx, loggerRepoMock.Object);
            var controllerMocked = new Mock<AbbonamentiController>(repo, loggerMock.Object);
            controllerMocked.CallBase = true;
            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
            return controllerMocked;
        }

        [Fact, TestOrder(1)]
        public async Task Crea_abbonamento_ok()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var abbonamento = new AbbonamentoUtenteApiModel()
            {
                IdCliente = Utils.ID_CLIENTE_TEST_1,
                IdTipoAbbonamento = Utils.ID_TIPO_ABBONAMENTO_1_CLIENTE_1,
                IngressiResidui = 50,
                Scadenza = DateTime.Now.AddMonths(3).Date,
                ScadenzaCertificato = DateTime.Now.AddMonths(2).Date,
                StatoPagamento = 1,
                UserId = Utils.USERID_DUMMY,
                DataInizioValidita = DateTime.Now.AddDays(-1).Date
            };
            var result = await controller.Object.AddAbbonamento(Utils.ID_CLIENTE_TEST_1, abbonamento);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            abbonamento.Id = id;
            s_abbonamento = abbonamento;
        }

        [Fact, TestOrder(2)]
        public async Task Get_abbonamento_ok()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = await controller.Object.GetAbbonamento(Utils.ID_CLIENTE_TEST_1, s_abbonamento.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(s_abbonamento);
        }

        [Fact, TestOrder(3)]
        public async Task Modify_abbonamento_ok()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            s_abbonamento.IdTipoAbbonamento = Utils.ID_TIPO_ABBONAMENTO_2_CLIENTE_1;
            s_abbonamento.IngressiResidui = 20;
            s_abbonamento.Scadenza = DateTime.Now.AddMonths(2).Date;
            s_abbonamento.ScadenzaCertificato = DateTime.Now.AddMonths(3).Date;
            s_abbonamento.StatoPagamento = 2;
            s_abbonamento.DataInizioValidita = DateTime.Now.AddDays(-10).Date;
            var result = await controller.Object.UpdateAbbonamento(Utils.ID_CLIENTE_TEST_1, s_abbonamento);
            result.Should().BeOfType<OkResult>();
        }

        [Fact, TestOrder(4)]
        public async Task Get_abbonamento_post_modifica_ok()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = await controller.Object.GetAbbonamento(Utils.ID_CLIENTE_TEST_1, s_abbonamento.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(s_abbonamento);
        }

        [Fact, TestOrder(5)]
        public async Task Crea_secondo_abbonamento_ok()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var abbonamento = new AbbonamentoUtenteApiModel()
            {
                IdCliente = Utils.ID_CLIENTE_TEST_1,
                IdTipoAbbonamento = Utils.ID_TIPO_ABBONAMENTO_1_CLIENTE_1,
                IngressiResidui = 10,
                Scadenza = DateTime.Now.AddMonths(3).Date,
                ScadenzaCertificato = DateTime.Now.AddMonths(2).Date,
                StatoPagamento = 1,
                UserId = Utils.USERID_DUMMY,
                DataInizioValidita = DateTime.Now.AddDays(-1).Date
            };
            var result = await controller.Object.AddAbbonamento(Utils.ID_CLIENTE_TEST_1, abbonamento);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            abbonamento.Id = id;
            s_abbonamento2 = abbonamento;
        }

        [Fact, TestOrder(6)]
        public void Get_abbonamenti_for_user()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetAbbonamentiForUser(Utils.ID_CLIENTE_TEST_1, Utils.USERID_DUMMY);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var abbonamenti = ((List<AbbonamentoViewModel>)okResult.Value).OrderByDescending(a=>a.Id).ToArray();
            abbonamenti.Should().HaveCountGreaterOrEqualTo(2);
            abbonamenti[0].Should().BeEquivalentTo(s_abbonamento2);
            abbonamenti[1].Should().BeEquivalentTo(s_abbonamento);
        }
    }
    */
}
