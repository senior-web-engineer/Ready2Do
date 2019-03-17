using Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Models.Utils;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers.API
{
    [Route("{cliente}/eventi")]
    public class EventFeedsController : ControllerBase
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger<BlobUploadController> _logger;
        private readonly SchedulesProxy _schedulesProxy;
        private readonly ClienteResolverServices _clienteResolver;

        public EventFeedsController(ILogger<BlobUploadController> logger,
                                    IOptions<AppConfig> apiOptions,
                                    ClienteResolverServices clienteResolver,
                                    SchedulesProxy schedulesProxy)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _schedulesProxy = schedulesProxy;
            _clienteResolver = clienteResolver;
        }


        [HttpGet("feeds")]
        public async Task<IActionResult> GetEvents([FromRoute(Name = "cliente")] string clientRoute, [FromQuery] string lid, [FromQuery] string start, [FromQuery] string end)
        {
            _logger.LogInformation($"Gettings events for [{clientRoute}], Location: {lid}, StartDate:{start}, EndDate: {end}");
            AuthTokenModel token;
            int idLocation = -1;
            int? idLocationNullable = null;
            if (string.IsNullOrEmpty(clientRoute))
            {
                return BadRequest();
            }
            if(int.TryParse(lid, out idLocation))
            {
                idLocationNullable = idLocation;
            }

            DateTime? startDate = start.FromIS8601();
            DateTime? endDate = end.FromIS8601();
            
            if (!startDate.HasValue)
            {
                return BadRequest();
            }
            if (!endDate.HasValue)
            {
                return BadRequest();
            }
            int idCliente = await _clienteResolver.GetIdClienteFromRouteAsync(clientRoute);
            var schedules = await _schedulesProxy.GetSchedulesAsync(idCliente, startDate.Value, endDate.Value, idLocationNullable);
            var result = schedules.MapToSchedulerEventViewModel();

            return Ok(result);
        }
    }
}
