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
    [Authorize]
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


        [HttpPost("{email}/confirmation")]
        [AllowAnonymous]
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
            await _clientiProvisioner.ProvisionClienteAsync(user.CreationToken, user.Id);
            return new OkResult();
        }

    }
}
