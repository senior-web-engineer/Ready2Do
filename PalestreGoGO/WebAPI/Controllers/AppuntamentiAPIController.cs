﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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

namespace PalestreGoGo.WebAPI.Controllers
{
    /// <summary>
    /// Gestione appuntamenti
    /// </summary>
    /// <seealso cref="https://palestregogo.visualstudio.com/MyFirstProject/_wiki?pagePath=%2FAppuntamenti"/>
    [Produces("application/json")]
    [Route("api/clienti/{idCliente:int}/schedules/{idSchedule:int}/appuntamenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppuntamentiAPIController : PalestreControllerBase
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
        private readonly ILogger<ClientiController> _logger;
        private readonly IAppuntamentiRepository _repositoryAppuntamenti;
        private readonly ISchedulesRepository _repositorySchedule;
        private readonly IConfiguration _config;

        public AppuntamentiAPIController(IConfiguration config,
                                 ILogger<ClientiController> logger,
                                 IAppuntamentiRepository repositoryAppuntamenti,
                                 ISchedulesRepository repositorySchedule)
        {
            this._config = config;
            this._logger = logger;
            this._repositoryAppuntamenti = repositoryAppuntamenti;
            this._repositorySchedule = repositorySchedule;
        }


        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppuntamentoForCurrentUser([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule)
        {
            AppuntamentoViewModel result = new AppuntamentoViewModel();
            //Leggiamo i dati sull'evento
            var schedule = await _repositorySchedule.GetScheduleAsync(idCliente, idSchedule);
            result.IdEvento = idSchedule;
            result.DataOra = string.Format("{0}T{1}Z", schedule.DataOraInizio.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), schedule.DataOraInizio.TimeOfDay.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            result.DurataMinuti = schedule.TipologiaLezione.Durata;
            result.Istruttore = schedule.Istruttore;
            result.MaxDataOraCancellazione = schedule.CancellabileFinoAl.HasValue
                                                    ? schedule.CancellabileFinoAl.Value.ToString("o", CultureInfo.InvariantCulture)
                                                    : schedule.DataOraInizio.ToString("0", CultureInfo.InvariantCulture);
            result.Nome = schedule.Title;
            result.NomeSala = schedule?.Location?.Nome;
            result.PostiDisponibili = schedule.PostiDisponibili;
            result.PostiResidui = schedule.PostiResidui.Value;

            //Verifichiamo se esiste un appuntamento per l'utente chiamante per l'evento
            if (User.Identity.IsAuthenticated)
            {
                var idUser = User.UserId();
                if (!string.IsNullOrWhiteSpace(idUser))
                {
                    var appuntuamento = await _repositoryAppuntamenti.GetAppuntamentoForScheduleAsync(idCliente, idSchedule, idUser);
                    if (appuntuamento != null)
                    {
                        result.IdAppuntamento = appuntuamento.Id;
                        result.DataOraIscrizione = appuntuamento.DataPrenotazione.ToString("o", CultureInfo.InvariantCulture);
                    }
                }
            }
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

            //Per creare un appuntamento l'utente deve avere un abbonamento al cliente            
            var appuntamento = new Appuntamenti();
            appuntamento.DataPrenotazione = DateTime.Now;
            appuntamento.IdCliente = idCliente;
            appuntamento.UserId = model.IdUtente;
            appuntamento.Note = model.Note;
            appuntamento.ScheduleId = model.IdEvento; //Siamo sicuri che questo sia per il cliente corrente??
            appuntamento.Id = await _repositoryAppuntamenti.TakeAppuntamentoAsync(idCliente, appuntamento);
            return Ok(appuntamento.Id);
        }

        [HttpPut("expiration/{userId}")]
        public async Task<IActionResult> HandleExpirationAppuntamentoDaConfermare([FromRoute(Name="idCliente")]int idCliente, [FromRoute(Name ="idSchedule")] int idSchedule,
                                        [FromRoute(Name ="userId")string userId)
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
                                                              [FromRoute(Name = "idSchedule")] int idAppuntamentoDaConfermare)
        {
            //Recuperare l'id del Workflow Run per invocare la terminazione (come avviene l'autorizzazione?)
            throw new NotImplementedException();
        }
            

        [HttpPost("guest")]
        public async Task<IActionResult> AddAppuntamentoForGuest([FromRoute]int idCliente, [FromBody] NuovoAppuntamentoGuestApiModel model)
        {
            if (model == null) return BadRequest();
            //Solo un amministratore può inserire appuntamenti per un utente GUEST
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            var appuntamento = new Appuntamenti();
            appuntamento.DataPrenotazione = DateTime.Now;
            appuntamento.IdCliente = idCliente;
            appuntamento.Note = model.Note;
            appuntamento.ScheduleId = model.IdEvento; //Siamo sicuri che questo sia per il cliente corrente??
            appuntamento.Nominativo = model.Nominativo; //Se abbiamo comunque l'utente, ci serve il nominativo?
            appuntamento.Id = await _repositoryAppuntamenti.AddAppuntamentoAsync(idCliente, appuntamento);
            return Ok(appuntamento.Id);
        }

        [HttpDelete("{idAppuntamento}")]
        public async Task<IActionResult> DeleteAppuntamento([FromRoute]int idCliente, [FromRoute(Name = "idSchedule")]int idSchedule, [FromRoute(Name = "idAppuntamento")] int idAppuntmento)
        {
            var appuntamento = await _repositoryAppuntamenti.GetAppuntamentoAsync(idCliente, idAppuntmento);
            if (appuntamento == null) return NotFound();
            bool authorized = false;
            if (User.CanManageStructure(idCliente)) { authorized = true; }
            if (!authorized && (!string.IsNullOrWhiteSpace(appuntamento.UserId)) && (appuntamento.UserId.Equals(User.UserId()))) { authorized = true; }
            if (!authorized) { return Forbid(); }
            await _repositoryAppuntamenti.CancelAppuntamentoAsync(idCliente, idAppuntmento);
            return Ok();
        }

    }
}