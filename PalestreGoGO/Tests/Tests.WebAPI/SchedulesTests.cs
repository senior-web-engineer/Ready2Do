//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using PalestreGoGo.DataAccess;
//using PalestreGoGo.DataModel;
//using PalestreGoGo.WebAPI.Controllers;
//using PalestreGoGo.WebAPI.Services;
//using PalestreGoGo.WebAPIModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Tests.Utils;
//using Xunit;
//using Xunit.Abstractions;

//namespace Tests.WebAPI
//{
//    public class SchedulesFixture
//    {
//        public ScheduleApiModel Entity;
//        public PalestreGoGoDbContext DbContext;
//    }

//    public class SchedulesTests : BaseWebApiTests, IClassFixture<SchedulesFixture>
//    {
//        private SchedulesFixture _fixture;

//        public SchedulesTests(ITestOutputHelper output, SchedulesFixture fixture) : base(output)
//        {
//            _fixture = fixture;
//            _fixture.DbContext = Utils.BuildDbContext();

//        }

//        private Mock<SchedulesAPIController> SetupController(ClaimsPrincipal principal)
//        {
//            var loggerMock = new Mock<ILogger<SchedulesAPIController>>();
//            var loggerRepoMock = new Mock<ILogger<SchedulesRepository>>();
//            var repo = new SchedulesRepository(_fixture.DbContext, loggerRepoMock.Object);
//            var controllerMocked = new Mock<SchedulesController>(loggerMock.Object, repo);
//            controllerMocked.CallBase = true;
//            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
//            return controllerMocked;
//        }

//        [Fact, TestOrder(1)]
//        public async Task Crea_Schedule()
//        {
//            var user = Utils.GetGlobalAdminUser();
//            var controller = SetupController(user);
//            var schedule = new ScheduleApiModel()
//            {
//                IdCliente = Utils.ID_CLIENTE_TEST_1,
//                CancellabileFinoAl = DateTime.Now.AddDays(3),
//                DataOraInizio = DateTime.Now.AddDays(5).Date,
//                IdLocation = Utils.ID_LOCATION_1_CLIENTE_1,
//                IdTipoLezione = Utils.ID_TIPO_LEZIONE_1_CLIENTE_1,
//                Istruttore = "Yogi",
//                Note = "Bubu non viene",
//                PostiDisponibili = 20
//            };
//            var result = await controller.Object.AddSchedule(Utils.ID_CLIENTE_TEST_1, schedule);
//            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
//            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
//            id.Should().BeGreaterThan(0);
//            _fixture.Entity = schedule;
//            _fixture.Entity.Id = id;
//        }

//        [Fact, TestOrder(2)]
//        public async Task Get_Schedule()
//        {
//            var user = Utils.GetGlobalAdminUser();
//            var controller = SetupController(user);
//            var result = await controller.Object.GetSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//            okResult.Value.Should().BeEquivalentTo(_fixture.Entity);
//        }

//        [Fact, TestOrder(3)]
//        public async Task Modify_Schedule()
//        {
//            var user = Utils.GetGlobalAdminUser();
//            var controller = SetupController(user);
//            _fixture.Entity.CancellabileFinoAl.AddDays(1);
//            _fixture.Entity.DataOraInizio.AddDays(1);
//            _fixture.Entity.IdLocation = Utils.ID_LOCATION_2_CLIENTE_1;
//            _fixture.Entity.Istruttore = "Orso Yogi";
//            _fixture.Entity.Note += "MODIFIED";
//            //_fixture.Entity.OraInizio.Add(TimeSpan.FromMinutes(30));
//            _fixture.Entity.PostiDisponibili += 10;
//            var result = await controller.Object.UpdateSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity);
//            var okResult = result.Should().BeOfType<OkResult>().Subject;
//        }


//        [Fact, TestOrder(4)]
//        public async Task Get_Schedule_post_modifica()
//        {
//            var user = Utils.GetGlobalAdminUser();
//            var controller = SetupController(user);
//            var result = await controller.Object.GetSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
//            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//            okResult.Value.Should().BeEquivalentTo(_fixture.Entity);
//        }

//        [Fact, TestOrder(5)]
//        public async Task Delete_Schedule_with_appointments()
//        {
//            var abbonamento = _fixture.DbContext.AbbonamentiUtenti.First(a => a.IdCliente == Utils.ID_CLIENTE_TEST_1 &&
//                                                                            a.UserId == Utils.USERID_DUMMY);
//            abbonamento.IngressiResidui.Should().BePositive();
//            short ingressiResiduiPre = abbonamento.IngressiResidui.Value;

//            _fixture.DbContext.Appuntamenti.Add(new Appuntamenti()
//            {
//                DataPrenotazione = DateTime.Now,
//                IdCliente = Utils.ID_CLIENTE_TEST_1,
//                ScheduleId = _fixture.Entity.Id.Value,
//                UserId = Utils.USERID_DUMMY
//            });
//            await _fixture.DbContext.SaveChangesAsync();
            
//            var user = Utils.GetGlobalAdminUser();
//            var controller = SetupController(user).Object;
//            await controller.DeleteSchedule(Utils.ID_CLIENTE_TEST_1, _fixture.Entity.Id.Value);
//            //Rileggiamo l'abbonamento
//            abbonamento = _fixture.DbContext.AbbonamentiUtenti.First(a => a.IdCliente == Utils.ID_CLIENTE_TEST_1 &&
//                                                                            a.UserId == Utils.USERID_DUMMY);
//            abbonamento.IngressiResidui.Should().Be(++ingressiResiduiPre);

//        }
//    }
//}
