using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Palestregogo.STS.Model;
using Palestregogo.STS.Model.Identity;
using Microsoft.AspNetCore.Identity;

using static System.Net.WebUtility;
using System.Net;
using Palestregogo.STS.Services;
using Microsoft.Extensions.Logging;

namespace STS.API
{
    //[Produces("application/json")]
    //[Route("api/Users")]
    //[Authorize(Policy = "UserManagementPolicy")]
    //public class UsersController : ControllerBase
    //{
    //    private readonly UserManager<AppUser> _userManager;
    //    private readonly IUserConfirmationService _confirmUserService;
    //    private readonly ILogger<UsersController> _logger;

    //    public UsersController(UserManager<AppUser> userManager, IUserConfirmationService confirmService, ILogger<UsersController> logger)
    //    {
    //        this._userManager = userManager;
    //        this._confirmUserService = confirmService;
    //        this._logger = logger;
    //    }


    //    [HttpPost]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> Register([FromBody]UserRegistrationModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return new BadRequestResult();
    //        }
    //        var user = new AppUser(model.Email, model.Nome, model.Cognome, model.Telefono, model.CorrelationTokoen);
    //        var result = await _userManager.CreateAsync(user, model.Password);
    //        if (!result.Succeeded)
    //        {
    //            return StatusCode((int)HttpStatusCode.InternalServerError);
    //        }
    //        _logger.LogDebug($"User with email: {user.Email} persisted");
    //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
    //        // Send an email with this link
    //        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    //        await _confirmUserService.EnqueueConfirmationMailRequest(new ConfirmationMailQueueMessage(user.UserName, UrlEncode(code), user.CreationToken));
    //        //Rileggiamo l'utente appena creato per recuperarne l'Id
    //        user = await _userManager.FindByNameAsync(user.UserName);
    //        _logger.LogInformation("Created a new account with password.");
    //        //Ritorniamo l'Id
    //        return new OkObjectResult(user.Id);
    //    }

    //    // GET: api/Users/ConfirmEmail
    //    [HttpGet("ConfirmEmail")]
    //    [AllowAnonymous]
    //    public async Task<IActionResult> ConfirmEmail([FromQuery]string userEmail, [FromQuery]string code)
    //    {
    //        if (userEmail == null || code == null)
    //        {
    //            return StatusCode((int)HttpStatusCode.BadRequest);
    //        }

    //        var user = await _userManager.FindByEmailAsync(userEmail);
    //        if (user == null)
    //        {
    //            return StatusCode((int)HttpStatusCode.BadRequest);
    //        }
    //        var result = await _userManager.ConfirmEmailAsync(user, code);
    //        if (!result.Succeeded)
    //        {
    //            return StatusCode((int)HttpStatusCode.BadRequest);
    //        }
    //        return new OkObjectResult(user.Id);
    //    }
    //}
}