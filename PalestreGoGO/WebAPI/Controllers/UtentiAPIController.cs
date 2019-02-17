using Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Business;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/utenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UtentiAPIController : APIControllerBase
    {
        private readonly ILogger<UtentiAPIController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _repositoryClienti;
        private readonly IUtentiRepository _repositoryUtenti;
        private readonly IAppuntamentiRepository _repositoryAppuntamenti;
        private readonly ISchedulesRepository _repositorySchedules;
        private readonly IRichiesteRegistrazioneRepository _repositoryRichRegistraz;
        private readonly UtentiBusiness _utentiBusiness;

        public UtentiAPIController(ILogger<UtentiAPIController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repositoryClienti,
                                 IAppuntamentiRepository repositoryAppuntamenti,
                                 ISchedulesRepository repositorySchedules,
                                 IUtentiRepository repositoryUtenti,
                                 IRichiesteRegistrazioneRepository repositoryRichRegistraz,
                                 UtentiBusiness utentiBusiness
                                 )
        {
            _logger = logger;
            _userManagementService = userManagementService;
            _repositoryClienti = repositoryClienti;
            _repositoryAppuntamenti = repositoryAppuntamenti;
            _repositoryUtenti = repositoryUtenti;
            _repositorySchedules = repositorySchedules;
            _utentiBusiness = utentiBusiness;
            _repositoryRichRegistraz = repositoryRichRegistraz;
        }

        /// <summary>
        /// Verifica se l'email specificata esiste già nel DB o meno
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Ritorna true se l'email non è stta ancora registrata, false se esiste già un utente con l'email specificata</returns>
        [HttpGet("checkemail")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }
            var user = await _userManagementService.GetUserByMailAsync(email);
            return Ok(user == null);
        }

        [HttpPost("send-confirm-email/{userEmail}")]
        public async Task<IActionResult> SendConfirmationEmail([FromRoute]string userEmail)
        {
            if(!User.Email().Equals(userEmail, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest();
            }
            var azUser = await _userManagementService.GetUserByIdAsync(User.UserId());
            await _userManagementService.SendConfirmationEmailAsync(userEmail, nome: azUser.Nome, cognome: azUser.Cognome);
            return Ok();
        }

        /// <summary>
        /// Conferma l'email dell'utente.
        /// Se invocato da un CLIENTE ritorna l'URL della struttura
        /// Se invocato da un UTENTE ordinario r
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>Ritorna l'url della homepage del cliente appena confermatp in caso di esito positivo</returns>
        [HttpPost("confirmation")]
        [AllowAnonymous]
        public async Task<ActionResult<UserConfirmationResultAPIModel>> ConfermaUtente([FromQuery]string email, [FromQuery]string code)
        {
            _logger.LogTrace($"ConfirmEmail -> Received request for user: [{email ?? "NULL"}], code: [{code ?? "NULL"}]");
            if (email == null || code == null)
            {
                return BadRequest();
            }
            var esitoConfirmation = await _userManagementService.ConfirmUserEmailAsync(email, code);
            if (!esitoConfirmation.Esito)
            {
                _logger.LogWarning($"ConfirmMail -> Failed validation for user: {email} with code: [{code}]");
                return BadRequest();
            }
            //var cliente = await _repository.GetByIdUserOwnerAsync(esitoConfirmation.IdUser);
            //if (cliente != null)
            //{
            //    esitoConfirmation.IdCliente = cliente.Id;
            //}
            ////TODO: Ritornare un CreatedAt con l'url del cliente?
            return Ok(esitoConfirmation);
        }

        #region Profilo Utente

        /// <summary>
        /// Ritorna le informazioni anagrafiche di un utente prendendole da B2C 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("profilo")]
        public async Task<ActionResult<UtenteDM>> GetProfilo()
        {
            string userId = GetCurrentUser()?.UserId();
            if (string.IsNullOrWhiteSpace(userId)) { return Forbid(); }
            AzureUser azUser = await _userManagementService.GetUserByIdAsync(userId);
            if (azUser == null)
            {
                _logger.LogError($"Impossibile trovare l'utente [{userId}]");
                return NotFound(); //Forse sarebbe più corretto un 500 invece di un 404
            }
            var result = new UtenteDM()
            {
                Nome = azUser.Nome,
                DisplayName = azUser.DisplayName,
                Email = azUser.Emails.FirstOrDefault(),
                TelephoneNumber = azUser.TelephoneNumber,
                Cognome = azUser.Cognome,
                UserId = azUser.Id
            };

            return Ok(result);

        }
        [HttpPut("profilo")]
        public async Task<IActionResult> SalvaProfilo([FromBody]UtenteInputDM profilo)
        {
            string userId = GetCurrentUser()?.UserId();
            if (string.IsNullOrWhiteSpace(userId)) { return Forbid(); }
            await _userManagementService.SaveProfileChangesAsync(userId, profilo);
            return Ok();
        }
        #endregion

        /// <summary>
        /// Invocata da un utente, ritorna i suoi dettagli
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<UtenteDetailsDM>> GetUserDetails([FromQuery(Name = "incAbb")] int? numAbbonamentiToIncl = 0,
                                                                        [FromQuery(Name = "apFrom")] string includeAppuntamentiFrom = null,
                                                                        [FromQuery(Name = "apTo")] string includeAppuntamentiTo = null,
                                                                        [FromQuery(Name = "incAppDaConf")] bool? includeAppuntamentiDaConfermare = false,
                                                                        [FromQuery(Name = "incCert")] bool? includeCertificati = false)
        {
            string userId = GetCurrentUser().UserId();
            DateTime? appuntamentiFrom = includeAppuntamentiFrom.FromIS8601();
            if (!string.IsNullOrEmpty(includeAppuntamentiFrom) && !appuntamentiFrom.HasValue) { return BadRequest(); /* formato data errato*/ }
            DateTime? appuntamentiTo = includeAppuntamentiTo.FromIS8601();
            if (!string.IsNullOrEmpty(includeAppuntamentiTo) && !appuntamentiTo.HasValue) { return BadRequest(); /* formato data errato*/ }

            UtenteDetailsDM result = new UtenteDetailsDM(userId);
            var clienti = await _repositoryUtenti.GetGlientiFollowedAsync(userId);
            if (clienti == null) { return Ok(result); }
            foreach (var c in clienti)
            {
                result.ClientiAssociati.Add(new ClienteAssociatoUtenteDM()
                {
                    Cliente = c,
                    DatiAssociazione = await _utentiBusiness.GetDettagliAssociazionoConCliente(c.IdCliente, userId, numAbbonamentiToIncl, appuntamentiFrom, appuntamentiTo,
                                                                                             includeAppuntamentiDaConfermare, includeCertificati)
                });
            }
            return Ok(result);
        }


        [HttpGet("{userId}/appuntamenti")]
        public async Task<ActionResult<IEnumerable<AppuntamentoUserApiModel>>> GetAppuntamentiForUser([FromRoute] string userId, [FromQuery] bool includePast = false)
        {
            IEnumerable<AppuntamentoUserApiModel> result = null;
            //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
            if (string.IsNullOrWhiteSpace(User.UserId()) || !User.UserId().Equals(userId))
            {
                return Forbid();
            }
            var appuntamenti = await _repositoryAppuntamenti.GetAppuntamentiUtenteAsync(User.UserId(), dtInizioSchedule: DateTime.Now);
            if (appuntamenti != null)
            {
                var idSchedules = appuntamenti.Select(a => a.ScheduleId).Distinct();
                var schedules = await _repositorySchedules.SchedulesLookupAsync(idSchedules, true);
                result = appuntamenti.Select(a => new AppuntamentoUserApiModel()
                {
                    Appuntamento = a,
                    Schedule = schedules.Single(s => s.Id.Equals(a.ScheduleId))
                });
            }
            return Ok(result);
        }

        /// <summary>
        /// Ritorna il profilo dell'utente così come salvato sull'Identity Provider (Azure B2C)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("me/ip")]
        public async Task<ActionResult<AzureUser>> GetMyUserProfileFromIdP()
        {
            string userId = GetCurrentUser()?.UserId();
            AzureUser azUser = await _userManagementService.GetUserByIdAsync(userId);
            return Ok(azUser);
        }

        //[HttpGet("{userId}/clientifollowed")]
        //public async Task<IActionResult> GetClientiFollowedForUserAsync([FromRoute] string userId)
        //{
        //    //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
        //    if (string.IsNullOrWhiteSpace(User.UserId()) || !User.UserId().Equals(userId))
        //    {
        //        return Forbid();
        //    }
        //    var result = new List<ClienteFollowedApiModel>();
        //    var clienti = await _repositoryUtenti.GetGlientiFollowedAsync(userId);
        //    for (int idx = 0; clienti != null && idx < clienti.Count; idx++)
        //    {
        //        result.Add(new ClienteFollowedApiModel()
        //        {
        //            IdCliente = clienti[idx].AnagraficaCliente.Id.Value,
        //            Nome = clienti[idx].AnagraficaCliente.Nome,
        //            DataFollowing = clienti[idx].DataFollowing,
        //            RagioneSociale = clienti[idx].AnagraficaCliente.RagioneSociale,
        //            AbbonamentoValido = clienti[idx].AbbonamentoValido
        //        });
        //    }
        //    return Ok(result);
        //}

        [HttpGet("follow/{idCliente:int}")]
        public async Task<IActionResult> UserFollowCliente([FromRoute] int idCliente)
        {
            string userId = User.UserId();
            //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }
            //Recupero i dati dell'utente da B2C
            var userInfo = await _userManagementService.GetUserByIdAsync(userId);
            bool result = await _repositoryUtenti.UserFollowClienteAsync(userId, idCliente);
            return Ok(result);
        }
    }
}

