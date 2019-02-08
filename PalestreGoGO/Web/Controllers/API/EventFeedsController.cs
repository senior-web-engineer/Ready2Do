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
using Web.Utils;

namespace Web.Controllers.API
{
    [Route("{cliente}/eventi")]
    public class EventFeedsController : ControllerBase
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger<BlobUploadController> _logger;
        private readonly SchedulesProxy _schedulesProxy;

        public EventFeedsController(ILogger<BlobUploadController> logger,
                                    IOptions<AppConfig> apiOptions,
                                    SchedulesProxy schedulesProxy)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _schedulesProxy = schedulesProxy;
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

            if (!HttpContext.Request.Headers.ContainsKey(Constants.CUSTOM_HEADER_TOKEN_AUTH))
            {
                return BadRequest();
            }
            var header = HttpContext.Request.Headers[Constants.CUSTOM_HEADER_TOKEN_AUTH];
            var headerValue = header.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                _logger.LogWarning("Unauthorized access to GetEvents(...)");
                return Unauthorized();
            }
            try
            {
                string json = SecurityUtils.DecryptStringFromBytes_Aes(Convert.FromBase64String(headerValue), Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
                token = JsonConvert.DeserializeObject<AuthTokenModel>(json);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Errore durante decodifica del token.");
                return Unauthorized();
            }
            if (token == null || !clientRoute.Equals(token.ClientRoute))
            {
                _logger.LogWarning("Unauthorized access to GetEvents(...)");
                return Unauthorized();
            }

            if (DateTime.Now.Subtract(token.CreationTime).TotalMinutes > _appConfig.AuthTokenDuration)
            {
                _logger.LogWarning("SASToken scaduto. {0}", token);
                return Unauthorized();
            }
            //var cliente = await WebAPIClient.GetClienteAsync(clientRoute, _appConfig.WebAPI.BaseAddress);
            var schedules = await _schedulesProxy.GetSchedulesAsync(token.IdCliente, startDate.Value, endDate.Value, idLocationNullable);
            var result = schedules.MapToSchedulerEventViewModel();

            return Ok(result);
        }
    }
}
