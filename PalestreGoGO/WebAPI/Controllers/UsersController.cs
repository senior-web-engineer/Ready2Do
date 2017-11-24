using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.WebAPI.Model;
using PalestreGoGo.WebAPI.Services;
using System.Net;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    using PalestreGoGo.IdentityModel;
    using static System.Net.WebUtility;

    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        //private readonly UserManager<AppUser> _userManager;
        //private readonly IUserConfirmationService _confirmUserService;
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiProvisioner _clientiProvisioner;

        public UsersController( IUsersManagementService userManagementService,
                                IClientiProvisioner clientiProvisioner,
                                ILogger<UsersController> logger)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
            this._clientiProvisioner = clientiProvisioner;
        }


        // Spostata la logica nel servizio UserManagementService, invocata dal controller dei Clienti

        //[HttpPost]
        //public async Task<IActionResult> RegisterUser([FromBody]NuovoUtenteViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestResult();
        //    }
        //    var user = new AppUser(model.Email, model.Nome, model.Cognome, model.Telefono, model.CorrelationToken);
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        return StatusCode((int)HttpStatusCode.InternalServerError);
        //    }
        //    _logger.LogDebug($"User with email: {user.Email} persisted");
        //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //    // Send an email with this link
        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    _logger.LogTrace($"RegisterUser-> generated code [{code}]for user: {user.UserName}");
        //    await _confirmUserService.EnqueueConfirmationMailRequest(new ConfirmationMailQueueMessage(user.UserName, code, user.CreationToken));
        //    //Rileggiamo l'utente appena creato per recuperarne l'Id
        //    user = await _userManager.FindByNameAsync(user.UserName);
        //    _logger.LogInformation("Created a new account with password.");
        //    //Ritorniamo l'Id
        //    return new OkObjectResult(user.Id);
        //}

        [HttpPost("{email}/confirmation")]
        public async Task<IActionResult> ConfirmEmail([FromRoute]string email, [FromBody]string code)
        {
            _logger.LogTrace($"ConfirmEmail -> Received request for user: [{email ?? "NULL"}], code: [{code ?? "NULL"}]");
            if (email == null || code == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
            bool esito =await _userManagementService.ConfirmUserAsync(email, code);
            if (!esito)
            {
                _logger.LogWarning($"ConfirmMail -> Failed validation for user: {email} with code: [{code}]");
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
            var user = await _userManagementService.GetUserByMailAsync(email);
            //Provisioning
            _clientiProvisioner.ProvisionCliente(user.CreationToken);
            return new OkResult();
        }
    }
}
