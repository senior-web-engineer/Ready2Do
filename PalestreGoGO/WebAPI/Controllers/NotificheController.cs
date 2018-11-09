using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/users/notifiche")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificheController : PalestreControllerBase
    {
        private readonly ILogger<NotificheController> _logger;
        private readonly INotificheRepository _notificheRepo;

        public NotificheController(ILogger<NotificheController> logger,
                                        INotificheRepository notificheRepo)
        {
            this._logger = logger;
            this._notificheRepo = notificheRepo;
        }

        [HttpGet("{filtro?}")]
        public async Task<IActionResult> GetNotificheAsync([FromRoute(Name = "filtro")] FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive, [FromQuery]int? idCliente = null)
        {
            string userId = User.UserId()?.ToString();
            var notifiche = await _notificheRepo.GetNotificheAsync(new UserReferenceDM(userId), idCliente, filtro);
            var result = Mapper.Map<IEnumerable<NotificaConTipoDM>, IEnumerable<NotificaConTipoApiModel>>(notifiche);
            return Ok(result);
        }

        /// <summary>
        /// Questa è un'operazioni amministrativa per cui lo userId non è quello dell'utente chiamante.
        /// L'utente chiamante deve però avere i permessi per ivnocare l'action
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notifica"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> AddNotificaAsync([FromRoute(Name = "userId")] string userId, NotificaApiModel notifica)
        {
            throw new NotImplementedException("Verificare la gestione della secuirty!");
            if (notifica == null) { return BadRequest(); }
            if (string.IsNullOrEmpty(userId)) { return BadRequest(); }
            if (notifica.UserRef == null)
            {
                notifica.UserRef = new UserReferenceApiModel(userId);
            }
            if (!userId.Equals(notifica.UserRef)) { return BadRequest(); }
            var dm = Mapper.Map<NotificaDM>(notifica);
            long idNotifica = await _notificheRepo.AddNotificaAsync(dm);
            return Ok(idNotifica);
        }

        [HttpPut("{idNotifica:int}")]
        public async Task<IActionResult> UpdateNotificaAsync([FromRoute(Name = "idNotifica")] long idNotifica, NotificaApiModel notifica)
        {
            string userId = User.UserId()?.ToString();
            if (string.IsNullOrWhiteSpace(userId)) { return Unauthorized(); }
            if (notifica == null) { return BadRequest(); }
            if(!userId.Equals(notifica.UserRef?.UserId)) { return BadRequest(); }
            await _notificheRepo.UpdateNotifica(idNotifica, notifica.DataPrimaVisualizzazione, notifica.DataDismissione);
            return Ok();
        }
    }
}
