using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess.Interfaces;
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
    [Authorize]
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
        private readonly IAppuntamentiRepository _repository;
        private readonly IConfiguration _config;

        public AppuntamentiController(IConfiguration config,
                                 ILogger<ClientiController> logger,
                                 IAppuntamentiRepository repository)
        {
            this._config = config;
            this._logger = logger;
            this._repository = repository;
        }


        [HttpPost()]
        public async Task<IActionResult> AddAppuntamento([FromRoute]int idCliente, [FromBody] NuovoEventoViewModel model)
        {
            if (model == null) return BadRequest();
            //Se è un amministratore può inserire appuntamenti anche per conto di altri utenti, altrimenti può inserire appuntamenti solo per se stesso
            if (!User.CanManageStructure(idCliente) && (User.UserId() != model.IdUtente)) { return BadRequest(); }
            //Per creare un appuntamento l'utente deve avere una bbonamento al cliente            
            var appuntamento = new Appuntamenti();
            appuntamento.DataPrenotazione = DateTime.Now;
            appuntamento.IdCliente = idCliente;
            appuntamento.IsGuest = false;
            appuntamento.UserId = model.IdUtente;
            appuntamento.Note = model.Note;
            appuntamento.ScheduleId = model.IdEvento; //Siamo sicuri che questo sia per il cliente corrente??
            appuntamento.Id = await _repository.AddAppuntamentoAsync(idCliente, appuntamento);
            return Ok(appuntamento.Id);
        }

        [HttpPost("/guest")]
        public async Task<IActionResult> AddAppuntamentoForGuest([FromRoute]int idCliente, [FromBody] NuovoEventoGuestViewModel model)
        {
            if (model == null) return BadRequest();
            //Solo un amministratore può inserire appuntamenti per un utente GUEST
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            var appuntamento = new Appuntamenti();
            appuntamento.DataPrenotazione = DateTime.Now;
            appuntamento.IdCliente = idCliente;
            appuntamento.IsGuest = true;
            appuntamento.UserId = model.IdUtente;
            appuntamento.Note = model.Note;
            appuntamento.ScheduleId = model.IdEvento; //Siamo sicuri che questo sia per il cliente corrente??
            appuntamento.Nominativo = model.Nominativo; //Se abbiamo comunque l'utente, ci serve il nominativo?
            appuntamento.Id = await _repository.AddAppuntamentoForGuestAsync(idCliente, appuntamento);
            return Ok(appuntamento.Id);
        }
    }
}