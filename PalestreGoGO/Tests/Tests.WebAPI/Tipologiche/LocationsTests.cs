using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Tests.WebAPI.Tipologiche
{
    public class LocationsTestsFixture
    {
        public LocationDM Location;
    }

    public class LocationsTests : BaseTipologicheTests, IClassFixture<LocationsTestsFixture>
    {

        private LocationsTestsFixture _locationFixture;

        private static Random s_random = new Random();

        public LocationsTests(ITestOutputHelper output, LocationsTestsFixture locationFixture) : base(output)
        {
            _locationFixture = locationFixture;
        }

        private Mock<LocationsAPIController> SetupController(ClaimsPrincipal principal)
        {
            var loggerMock = new Mock<ILogger<LocationsAPIController>>();
            var dbCtx = Utils.BuildDbContext();
            var repo = new LocationsRepository(dbCtx);
            var controllerMocked = new Mock<LocationsAPIController>(repo, loggerMock.Object);
            controllerMocked.CallBase = true;
            controllerMocked.Setup(x => x.GetCurrentUser()).Returns(principal);
            return controllerMocked;
        }

        [Fact, TestOrder(1)]
        public void Crea_location()
        {
            output.WriteLine("Executing Crea_location ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _locationFixture.Location = new LocationApiModel()
            {
                Nome = "Sala Rossa",
                Descrizione = "Sala rossa per yoga",
                CapienzaMax = 50
            };
            var result = controller.Object.Create(Utils.ID_CLIENTE_TEST_1, _locationFixture.Location);
            var okResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var id = okResult.Value.Should().BeAssignableTo<int>().Subject;
            id.Should().BeGreaterThan(0);
            _locationFixture.Location.Id = id;
        }


        [Fact, TestOrder(3)]
        public void Get_location()
        {
            output.WriteLine("Executing Get_location...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _locationFixture.Location.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var location = okResult.Value.Should().BeAssignableTo<LocationApiModel>().Subject;
            location.Id.Should().Be(_locationFixture.Location.Id);
            location.Nome.Should().Be(_locationFixture.Location.Nome);
            location.Descrizione.Should().Be(_locationFixture.Location.Descrizione);
            location.CapienzaMax.Should().Be(_locationFixture.Location.CapienzaMax);
        }

        [Fact, TestOrder(5)]
        public void Modify_location()
        {
            output.WriteLine("Executing Modify_location ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            _locationFixture.Location.Nome = _locationFixture.Location.Nome + "-MODIFIED";
            _locationFixture.Location.Descrizione = _locationFixture.Location.Descrizione + "-MODIFIED";
            _locationFixture.Location.CapienzaMax = 90;
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1, _locationFixture.Location);
            var okResult = result.Should().BeOfType<OkResult>().Subject;
        }

        [Fact, TestOrder(6)]
        public void Modify_location_wrongTenant()
        {
            output.WriteLine("Executing Modify_location_wrongTenant ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Modify(Utils.ID_CLIENTE_TEST_1 + 10, _locationFixture.Location);
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact, TestOrder(7)]
        public void Get_location_modified()
        {
            output.WriteLine("Executing Get_location_modified ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_1, _locationFixture.Location.Id.Value);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tipoLezione = okResult.Value.Should().BeAssignableTo<LocationApiModel>().Subject;
            tipoLezione.Id.Should().Be(_locationFixture.Location.Id);
            tipoLezione.Nome.Should().Be(_locationFixture.Location.Nome);
            tipoLezione.Descrizione.Should().Be(_locationFixture.Location.Descrizione);
            tipoLezione.CapienzaMax.Should().Be(_locationFixture.Location.CapienzaMax);
        }

        [Fact, TestOrder(20)]
        public void Get_location_cliente_errato()
        {
            output.WriteLine("Executing Get_location_cliente_errato ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetOne(Utils.ID_CLIENTE_TEST_2, Utils.ID_CLIENTE_TEST_1_TIPO_LEZIONE1);
            var okResult = result.Should().BeOfType<BadRequestResult>().Subject;
        }


        [Fact, TestOrder(40)]
        public void Get_all_locations()
        {
            output.WriteLine("Get_all_locations ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.GetAll(Utils.ID_CLIENTE_TEST_1);
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var locations = okResult.Value.Should().BeAssignableTo<IEnumerable<LocationApiModel>>().Subject;
            locations.Count().Should().BeGreaterThan(0);
            var location = locations.Should().ContainSingle(tl => tl.Id == _locationFixture.Location.Id).Subject;
            location.Id.Should().Be(_locationFixture.Location.Id);
            location.Nome.Should().Be(_locationFixture.Location.Nome);
            location.Descrizione.Should().Be(_locationFixture.Location.Descrizione);
            location.CapienzaMax.Should().Be(_locationFixture.Location.CapienzaMax);
        }

        [Fact, TestOrder(50)]
        public void Delete_location()
        {
            output.WriteLine("Executing Delete_location ...");
            var user = Utils.GetGlobalAdminUser();
            var controller = SetupController(user);
            var result = controller.Object.Delete(Utils.ID_CLIENTE_TEST_1, _locationFixture.Location.Id.Value);
            var okResult = result.Should().BeOfType<NoContentResult>().Subject;
        }
    }
}
