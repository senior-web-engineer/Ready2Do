using System;
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

namespace PalestreGoGo.WebAPI.Controllers
{
    /// <summary>
    /// Gestione appuntamenti
    /// </summary>
    /// <seealso cref="https://palestregogo.visualstudio.com/MyFirstProject/_wiki?pagePath=%2FAppuntamenti"/>
    [Produces("application/json")]
    [Route("api/clienti/{idCliente:int}/appuntamenti/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppuntamentiController : PalestreControllerBase
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

        public AppuntamentiController(IConfiguration config,
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
        public async Task<IActionResult> GetAppuntamento([FromRoute]int idCliente, [FromQuery]int idEvento)
        {
            AppuntamentoViewModel result = new AppuntamentoViewModel();
            //Leggiamo i dati sull'evento
            var schedule = await _repositorySchedule.GetScheduleAsync(idCliente, idEvento);
            result.IdEvento = idEvento;
            result.DataOra = string.Format("{0}T{1}Z", schedule.Data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), schedule.OraInizio.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
            result.DurataMinuti = schedule.TipologiaLezione.Durata;
            result.Istruttore = schedule.Istruttore;
            result.MaxDataOraCancellazione = schedule.CancellabileFinoAl.ToString("o", CultureInfo.InvariantCulture);
            result.Nome = schedule.Title;
            result.NomeSala = schedule?.Location?.Nome;
            result.PostiDisponibili = schedule.PostiDisponibili;
            result.PostiResidui = schedule.PostiResidui.Value;

            //Verifichiamo se esiste un appuntamento per l'utente chiamante per l'evento
            if (User.Identity.IsAuthenticated)
            {
                var idUser = User.UserId();
                if (idUser.HasValue)
                {
                    var appuntuamento = await _repositoryAppuntamenti.GetAppuntamentoForScheduleAsync(idCliente, idEvento, idUser.Value);
                    if (appuntuamento != null)
                    {
                        result.IdAppuntamento = appuntuamento.Id;
                        result.DataOraIscrizione = appuntuamento.DataPrenotazione.ToString("o", CultureInfo.InvariantCulture);
                    }
                }
            }
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> AddAppuntamento([FromRoute]int idCliente, [FromBody] NuovoAppuntamentoApiModel model)
        {
            if (model == null) return BadRequest();
            //Se è un amministratore può inserire appuntamenti anche per conto di altri utenti, altrimenti può inserire appuntamenti solo per se stesso
            if (!User.CanManageStructure(idCliente) && (User.UserId() != model.IdUtente)) { return BadRequest(); }
            //Per creare un appuntamento l'utente deve avere una bbonamento al cliente            
            var appuntamento = new Appuntamenti();
            appuntamento.DataPrenotazione = DateTime.Now;
            appuntamento.IdCliente = idCliente;
            appuntamento.UserId = model.IdUtente;
            appuntamento.Note = model.Note;
            appuntamento.ScheduleId = model.IdEvento; //Siamo sicuri che questo sia per il cliente corrente??
            appuntamento.Id = await _repositoryAppuntamenti.AddAppuntamentoAsync(idCliente, appuntamento);
            return Ok(appuntamento.Id);
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

       [HttpDelete("{id}")]
       public async Task<IActionResult> DeleteAppuntamento([FromRoute]int idCliente, [FromRoute] int idAppuntmento)
        {
            var appuntamento = await _repositoryAppuntamenti.GetAppuntamentoAsync(idCliente, idAppuntmento);
            if (appuntamento == null) return NotFound();
            bool authorized = false;
            if (User.CanManageStructure(idCliente)) { authorized = true; }
            if (!authorized && (appuntamento.UserId.HasValue) && (appuntamento.UserId.Value.Equals(User.UserId()))) { authorized = true; }
            if (!authorized) { return Forbid(); }

            await _repositoryAppuntamenti.CancelAppuntamentoAsync(idCliente, idAppuntmento);
            return Ok();
        }
    }
}