using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    /// <summary>
    /// Gestione appuntamenti
    /// </summary>
    /// <seealso cref="https://palestregogo.visualstudio.com/MyFirstProject/_wiki?pagePath=%2FAppuntamenti"/>
    [Produces("application/json")]
    [Route("api/clienti/{idCliente:int}/schedules/{idSchedule:int}/appuntamenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppuntamentiAPIController : APIControllerBase
    {

        /* 
         1. Get -> Ritorna i dettagli di un appuntamento 
                INOCABILE DA:
                    - Gestore Palestra
                    - Gestore dello Schedule (per ora non lo gestiamo)
                    - Utente owner dell'appuntamento:
         2. Post -> Crea un nuovo appuntamento
                INVOCABILE DA:
                    - utente registrato 
         */
        private readonly ILogger<AppuntamentiAPIController> _logger;
        private readonly IAppuntamentiRepository _repositoryAppuntamenti;
        private readonly IClientiRepository _clientiRepository;
        private readonly ISchedulesRepository _repositorySchedule;
        private readonly IConfiguration _config;
        private readonly LogicAppsClient _clientLogicApp;

        public AppuntamentiAPIController(IConfiguration config,
                                 ILogger<AppuntamentiAPIController> logger,
                                 IAppuntamentiRepository repositoryAppuntamenti,
                                 IClientiRepository clientiRepository,
                                 ISchedulesRepository repositorySchedule,
                                 LogicAppsClient clientLogicApp)
        {
            _config = config;
            _logger = logger;
            _repositoryAppuntamenti = repositoryAppuntamenti;
            _repositorySchedule = repositorySchedule;
            _clientiRepository = clientiRepository;
            _clientLogicApp = clientLogicApp;
        }


        /// <summary>
        /// Se invocata da un gestore ritorna tutti gli appuntamenti per lo Schedule (gli utenti iscritti)
        /// Se invece il chiamante è un utente normale ritorna solo il suo appuntamento se esiste
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <param name="page">Numero di pagina di dati corrente</param>
        /// <param name="pageSize">Numero di items per pagina</param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AppuntamentoDM>>> GetAppuntamentiForSchedule([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule)
        {
            bool isGestore = GetCurrentUser().CanManageStructure(idCliente);
            if (!isGestore)
            {
                //Se non è il gestore ritorno l'eventuale appuntamento esistente per l'utente
                var appuntamento = await _repositoryAppuntamenti.GetAppuntamentoForUserAsync(idCliente, idSchedule, GetCurrentUser().UserId());
                IEnumerable<AppuntamentoDM> result = appuntamento != null ? new AppuntamentoDM[] { appuntamento } : null;
                return Ok(result);
            }
            else
            {
                //Se è il gestore ritorno tutti gli appuntamenti (per ora non paginiamo
                var result = await _repositoryAppuntamenti.GetAppuntamentiScheduleAsync(idCliente, idSchedule, false);
                return Ok(result);
            }
        }


        /// <summary>
        /// Si il chiamante è un gestore, ritorna tutti gli appuntamenti da confermare per lo schedule specificato.
        /// Se il chiamante è un utente normale ritorna SOLO il suo appuntamento da confermare (e solo alcune informazioni)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        [HttpGet("unconfirmed")]
        public async Task<ActionResult<IEnumerable<AppuntamentoDaConfermareDM>>> GetAppuntamentiNonConfermatiForSchedule([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule)
        {
            //Solo il gestore può invocare questa API
            if (!GetCurrentUser().CanManageStructure(idCliente))
            {
                var app = await _repositoryAppuntamenti.GetAppuntamentoDaConfermareForUserAsync(idCliente, idSchedule, GetCurrentUser().UserId());
                return Ok(app);
            }
            else
            {
                //Se è il gestore ritorno tutti gli appuntamenti (per ora non paginiamo)
                var result = await _repositoryAppuntamenti.GetAppuntamentiDaConfermareScheduleAsync(idCliente, idSchedule, false);
                return Ok(result);
            }
        }

        /// <summary>
        /// Ritorna gli iscritti in waitlist
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        [HttpGet("waitlist")]
        public async Task<ActionResult<IEnumerable<WaitListRegistrationDM>>> GetWaitListForSchedule([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule)
        {
            //Solo il gestore può invocare questa API
            if (!GetCurrentUser().CanManageStructure(idCliente))
            {
                return Ok(await _repositoryAppuntamenti.GetWaitListRegistrationsScheduleAsync(idCliente, idSchedule, GetCurrentUser().UserId(), false, false));
            }

            //Se è il gestore ritorno tutti gli appuntamenti (per ora non paginiamo)
            var result = await _repositoryAppuntamenti.GetWaitListRegistrationsScheduleAsync(idCliente, idSchedule, null, false, false);
            return Ok(result);
        }


        /// <summary>
        /// Crea un nuovo appuntamento per l'utente corrente rappresentato dal Token di autorizzazione utilizzato.
        /// Se l'utente ha più di un abbonamento valido e compatibile con l'evento, deve indicare quale utilizzare
        /// valorizzando la proprietà IdAbbonamento.
        /// Se l'utente ha un solo abbonamento valido e compatibile con l'evento, l'IdAbbonamento è opzionale.
        /// Se invece l'utente NON ha un abbonamento valido, l'appuntamento viene preso come da Confermare
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> TakeAppuntamentoForCurrentUser([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule,
                                                                                    [FromBody] NuovoAppuntamentoApiModel model)
        {
            if (model == null) return BadRequest();
            if (model.IdEvento != idSchedule) return BadRequest();
            //Se è un amministratore può inserire appuntamenti anche per conto di altri utenti, altrimenti può inserire appuntamenti solo per se stesso
            if (!User.CanManageStructure(idCliente) && (!User.UserId().Equals(model.IdUtente))) { return BadRequest(); }
            //Startiamo la Logic App che gestisce il Timeout se non è stato specificato un IdAbbonamento
            try
            {
                string logiAccInfos = null;
                if (!model.IdAbbonamento.HasValue)
                {
                    int timeoutMinutes = 0;
                    string minutes = await _clientiRepository.GetPreferenzaCliente(idCliente, "APPUNTAMENTIDACONFERMARE.EXPIRATION.WINDOW.MINUTES");
                    if (!string.IsNullOrWhiteSpace(minutes) && !int.TryParse(minutes, out timeoutMinutes))
                    {
                        //Se non è configurata una preferenza sul DB, usiamo il default del file di configurazione
                        timeoutMinutes = _config.GetValue<int>("Azure__LogicApps__AppuntamentiDaConfermareHandler__DefaultTimeoutMinutes");
                    }
                    logiAccInfos = await _clientLogicApp.StartAppForAppuntamentoDaConfermare(idCliente, idSchedule, model.IdUtente, timeoutMinutes);
                }
                var appuntamento = await _repositoryAppuntamenti.TakeAppuntamentoAsync(idCliente, model.IdUtente, idSchedule, model.IdAbbonamento, model.Note, null, logiAccInfos);
                return Ok(appuntamento);
            }catch(Exception exc)
            {
                Log.Error(exc, "Errore durante il tentativo di Presa Appuntamento. idCliente: {IdCliente}, idSchedule: {IdSchedule}, appuntamento: {@Model}", idCliente, idSchedule, model);
                throw;
            }
        }

        [HttpPut("expiration/{userId}")]
        public async Task<IActionResult> HandleExpirationAppuntamentoDaConfermare([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idSchedule")] int idSchedule,
                                        [FromRoute(Name = "userId")]string userId)
        {
            //Aggiornare lo stato sul DB
            //Notificare all'utente ed al gestore la scadenza
            throw new NotImplementedException();
        }

        /// <summary>
        /// Trasforma un AppuntamentoDaConfermare in un Appuntamento a tutti gli effetti (sempre che l'utente soddisfi i requisiti al momento dell'invocazione)
        /// Termina la LogicApp avviata in fase di creazione dell'AppuntamentoDaConfermare per gestire il timeout della conferma.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <param name="idAppuntamentoDaConfermare"></param>
        /// <returns></returns>
        [HttpPost("conferma/{id:int}")]
        public async Task<IActionResult> ConfermaAppuntamento([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idSchedule")] int idSchedule,
                                                              [FromRoute(Name = "id")] int idAppuntamentoDaConfermare)
        {
            //Recuperare l'id del Workflow Run per invocare la terminazione (come avviene l'autorizzazione?)
            int idAppuntamento = await _repositoryAppuntamenti.AppuntamentoDaConfermareConferma(idCliente, idSchedule, idAppuntamentoDaConfermare);
            return Ok(idAppuntamento);
        }

        [HttpPost("rifiuta/{id:int}")]
        public async Task<IActionResult> RifiutaAppuntamento([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idSchedule")] int idSchedule,
                                                              [FromRoute(Name = "id")] int idAppuntamentoDaConfermare, [FromRoute(Name ="motivo")]string motivazione = null)
        {
            //Recuperare l'id del Workflow Run per invocare la terminazione (come avviene l'autorizzazione?)
            await _repositoryAppuntamenti.AppuntamentoDaConfermareRifiuta(idCliente, idSchedule, idAppuntamentoDaConfermare, motivazione);
            return Ok();
        }


        [HttpPost("guest")]
        public async Task<IActionResult> AddAppuntamentoForGuest([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")] int idSchedule, [FromBody] NuovoAppuntamentoGuestApiModel model)
        {
            if (model == null) return BadRequest();
            //Solo un amministratore può inserire appuntamenti per un utente GUEST
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            var appuntamento = await _repositoryAppuntamenti.TakeAppuntamentoAsync(idCliente, null, idSchedule, null, model.Note, model.Nominativo, null);
            return Ok(appuntamento);
        }

        /// <summary>
        /// Questa API cancella, se esiste, altrimenti ritorna un 404, l'appuntamento per l'utente chiamante
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <returns></returns>
        [HttpDelete()]
        public async Task<IActionResult> DeleteAppuntamento([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule)
        {
            var userId = GetCurrentUser().UserId();
            var appuntamento = await _repositoryAppuntamenti.GetAppuntamentoForUserAsync(idCliente, idSchedule, userId);
            if (appuntamento == null)
            {
                var appuntamentoDaConfermare = (await _repositoryAppuntamenti.GetAppuntamentoDaConfermareForUserAsync(idCliente, idSchedule, userId)).SingleOrDefault();
                if (appuntamentoDaConfermare == null) { return NotFound(); }
                else
                {
                    var timoutMgrPayload = await _repositoryAppuntamenti.CancelAppuntamentoDaConfermareAsync(idCliente, appuntamentoDaConfermare.Id.Value);
                    //TODO: Terminare il TimeoutManager (chiamata REST per interrompere la logic app)
                    //Al momento la LogiApp non viene interrotta per cui cercherà inutilmante di invocare il timeout. 
                    //L'operazione non produrrà effetti essendo già stata cancellata
                }
            }
            else
            {
                await _repositoryAppuntamenti.CancelAppuntamentoAsync(idCliente, appuntamento.Id.Value);
            }
            return Ok();
        }

        /// <summary>
        /// Questa operazione è invocabile solo dal gestore per cancellare l'appuntamento di un altro utente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <param name="idAppuntamento"></param>
        /// <returns></returns>
        [HttpDelete("{idAppuntamento}")]
        public async Task<IActionResult> DeleteAppuntamentoAdmin([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule, [FromRoute(Name = "idAppuntamento")] int idAppuntamento)
        {
            if (!User.CanManageStructure(idCliente))
            {
                Log.Warning("Tentativo NON AUTORIZZATO di cancellare l'appountamento [{idAppuntamento}] per l'evento [{idEvento}]. Caller Id: {userId}",
                    idAppuntamento, idSchedule, GetCurrentUser().UserId());
                return Forbid();
            }
            await _repositoryAppuntamenti.CancelAppuntamentoAsync(idCliente, idAppuntamento);
            Log.Information("Cancellato l'appountamento [{idAppuntamento}] per l'evento [{idEvento}]. Caller Id: {userId}",
                    idAppuntamento, idSchedule, GetCurrentUser().UserId());
            return Ok();
        }
    }
}