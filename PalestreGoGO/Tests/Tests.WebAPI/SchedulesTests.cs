using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Tests.WebAPI
{
    public class SchedulesFixture
    {
        public ScheduleViewModel Entity;
    }

    public class SchedulesTests : BaseWebApiTests, IClassFixture<SchedulesFixture>
    {
        private SchedulesFixture _fixture;

        public SchedulesTests(ITestOutputHelper output, SchedulesFixture fixture) : base(output)
        {
            _fixture = fixture;
        }

        private Mock<SchedulesController> SetupController(ClaimsPrincipal principal)
        {
            var loggerMock = new Mock<ILogger<SchedulesController>>();
            var loggerRepoMock = new Mock<ILogger<SchedulesRepository>>();
            var dbCtx = Utils.BuildDbContext();
            var repo = new SchedulesRepository(dbCtx,loggerRepoMock.Object);
            var controllerMocked = new Mock<SchedulesController>(loggerMock.Object, repo);
            controllerMocked.CallBase = true;
            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
            return controllerMocked;
        }

        [Fact, TestOrder(1)]
        public async Task Crea_Schedule()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var schedule = new ScheduleViewModel()
            {
                IdCliente = Utils.ID_CLIENTE_TEST_1,
                CancellabileFinoAl = DateTime.Now.AddDays(3),
                Data = DateTime.Now.AddDays(5).Date,
                IdLocation = Utils.ID_LOCATION_1_CLIENTE_1,
                IdTipoLezione = Utils.ID_TIPO_LEZIONE_1_CLIENTE_1,
                Istruttore = "Yogi",
                Note = "Bubu non viene",
                OraInizio = new TimeSpan(19,0,0),
                PostiDisponibili = 20
            };
            var result = await controller.Object.AddSchedule(Utils.ID_CLIENTE_TEST_1, schedule);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            _fixture.Entity = schedule;
            _fixture.Entity.Id = id;
        }

        [Fact, TestOrder(2)]
        public async Task Get_Schedule()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = await controller.Object.GetSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(_fixture.Entity);
        }

        [Fact, TestOrder(3)]
        public async Task Modify_Schedule()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _fixture.Entity.CancellabileFinoAl.AddDays(1);
            _fixture.Entity.Data.AddDays(1);
            _fixture.Entity.IdLocation = Utils.ID_LOCATION_2_CLIENTE_1;
            _fixture.Entity.Istruttore = "Orso Yogi";
            _fixture.Entity.Note += "MODIFIED";
            _fixture.Entity.OraInizio.Add(TimeSpan.FromMinutes(30));
            _fixture.Entity.PostiDisponibili += 10;
            var result = await controller.Object.UpdateSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity);
            var okResult = result.Should().BeOfType<OkResult>().Subject;
        }


        [Fact, TestOrder(4)]
        public async Task Get_Schedule_post_modifica()
        {
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = await controller.Object.GetSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(_fixture.Entity);
        }
    }
}
