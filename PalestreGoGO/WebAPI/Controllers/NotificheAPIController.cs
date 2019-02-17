using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/users/notifiche")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificheAPIController : APIControllerBase
    {
        private readonly ILogger<NotificheAPIController> _logger;
        private readonly INotificheRepository _notificheRepo;

        public NotificheAPIController(ILogger<NotificheAPIController> logger,
                                        INotificheRepository notificheRepo)
        {
            this._logger = logger;
            this._notificheRepo = notificheRepo;
        }

        /// <summary>
        /// Ritorna le notifiche per l'utente chiamante per tutti i clienti a meno che non sia specificato un idCliente
        /// </summary>
        /// <param name="filtro">Indica il tipo di notifiche desiderata (Attive, Tutte, ecc...)</param>
        /// <param name="idCliente">Se specificato l'API ritorna solo le notifiche relative a quel Cliente</param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<NotificaConTipoDM>>> GetNotificheForCurrentUserAsync([FromQuery(Name = "filtro")] string filtro = "SoloAttive", [FromQuery]int? idCliente = null)
        {
            FiltroListaNotificheDM filter;
            if(!Enum.TryParse<FiltroListaNotificheDM>(filtro, out filter))
            {
                filter = FiltroListaNotificheDM.SoloAttive;
            }

            string userId = User.UserId()?.ToString();
            var result = await _notificheRepo.GetNotificheAsync(userId, idCliente, filter);
            return Ok(result);
        }

        /// <summary>
        /// Questa è un'operazioni amministrativa per cui lo userId non è quello dell'utente chiamante.
        /// L'utente chiamante deve però avere i permessi per invocare l'action
        /// </summary>
        /// <param name="userId">Utente destinatario della notifica</param>
        /// <param name="notifica">Notifica vera e propria </param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> AddNotificaForUserAsync([FromRoute(Name = "userId")] string userId, NotificaDM notifica)
        {
            if (notifica == null) { return BadRequest(); }
            if (string.IsNullOrEmpty(userId)) { return BadRequest(); }
            if (string.IsNullOrWhiteSpace(notifica.UserId)) { notifica.UserId = userId; }
            if (!userId.Equals(notifica.UserId)) { return BadRequest(); }
            //
            long idNotifica = await _notificheRepo.AddNotificaAsync(notifica);
            return Ok(idNotifica);
        }

        /// <summary>
        /// Aggiorna lo stato di una notifica dell'utente chiamante
        /// </summary>
        /// <param name="idNotifica"></param>
        /// <param name="dataVisualizzazione"></param>
        /// <param name="dataDismiss"></param>
        /// <returns></returns>
        [HttpPut("{idNotifica:int}")]
        public async Task<IActionResult> UpdateNotificaAsync([FromRoute(Name = "idNotifica")] long idNotifica, [FromQuery(Name = "v")] DateTime? dataVisualizzazione = null,
                                                             [FromQuery(Name = "d")] DateTime? dataDismiss = null)
        {
            string userId = User.UserId()?.ToString();
            if (string.IsNullOrWhiteSpace(userId)) { return Unauthorized(); }
            if (!dataDismiss.HasValue && !dataVisualizzazione.HasValue) { return BadRequest(); }
            await _notificheRepo.UpdateNotifica(userId, idNotifica, dataVisualizzazione, dataDismiss);
            return Ok();
        }
    }
}
