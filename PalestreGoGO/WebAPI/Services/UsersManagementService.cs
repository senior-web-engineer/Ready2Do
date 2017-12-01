using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserConfirmationService _confirmUserService;
        private readonly ILogger<UsersManagementService> _logger;
        private readonly IClientiRepository _repositoryClienti;

        public UsersManagementService(UserManager<AppUser> userManager,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiRepository repositoryClienti)
        {
            this._userManager = userManager;
            this._confirmUserService = confirmService;
            this._repositoryClienti = repositoryClienti;
            this._logger = logger;
        }

        public async Task<bool> ConfirmUserAsync(string username, string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"ConfirmMail -> Failed validation for user: {username} with code: [{code}]");
                return false;
            }
            var claims = await _userManager.GetClaimsAsync(user);
            //Se è un owner ==> facciamo il provisioning del cliente
            if (claims.Any(c => c.Type.Equals(Constants.ClaimStructureOwned)))
            {
                
            }
            _logger.LogInformation($"ConfirmMail -> Successfully validated user {username}");
            return true;
        }

        public Task<AppUser> GetUserByMailAsync(string email)
        {
            return this._userManager.FindByEmailAsync(email);
        }

        public Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return this._userManager.FindByNameAsync(username);
        }

        public async Task<Guid> RegisterOwnerAsync(AppUser user, string password, string idCliente)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentNullException(nameof(user.UserName));
            if (string.IsNullOrWhiteSpace(user.FirstName)) throw new ArgumentNullException(nameof(user.FirstName));
            if (string.IsNullOrWhiteSpace(user.LastName)) throw new ArgumentNullException(nameof(user.LastName));
            if (string.IsNullOrWhiteSpace(user.PhoneNumber)) throw new ArgumentNullException(nameof(user.PhoneNumber));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(idCliente)) throw new ArgumentNullException(nameof(idCliente));


            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).Aggregate((i, j) => $"{i} | {j} ");
                throw new ApplicationException($"Errore durante il salvataggio dell'utente. Errors: {errors}");
            }
            _logger.LogDebug($"User with email: {user.Email} persisted");
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogTrace($"RegisterUser-> generated code [{code}]for user: {user.UserName}");
            await _confirmUserService.EnqueueConfirmationMailRequest(new ConfirmationMailQueueMessage(user.UserName, code, user.CreationToken));
            //Rileggiamo l'utente appena creato 
            user = await _userManager.FindByNameAsync(user.UserName);
            // Rendiamo l'utente OWNER della struttura (associando il claim all'utente)
            await _userManager.AddClaimAsync(user, new Claim(Constants.ClaimStructureOwned, idCliente));

            _logger.LogInformation($"Created a new account with password. UserId: {user.Id}");

            return user.Id;
        }

    }
}
