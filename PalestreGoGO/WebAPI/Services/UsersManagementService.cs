using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Model;
using PalestreGoGo.WebAPI.ViewModel;
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
        private readonly IClientiProvisioner _clientiProvisioner;

        public UsersManagementService(UserManager<AppUser> userManager,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._confirmUserService = confirmService;
            this._clientiProvisioner = clientiProvisioner;
        }

        public async Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return new UserConfirmationViewModel(false);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"ConfirmUserAsync -> Failed validation for user: {username} with code: [{code}]");
                return new UserConfirmationViewModel(false);
            }
            var claims = await _userManager.GetClaimsAsync(user);
            //Se è un owner ==> facciamo il provisioning del cliente
            if (claims.Any(c => c.Type.Equals(Constants.ClaimStructureOwned)))
            {
                await _clientiProvisioner.ProvisionClienteAsync(user.CreationToken, user.Id);
            }
            _logger.LogInformation($"ConfirmUserAsync -> Successfully validated user {username}");

            return new UserConfirmationViewModel(user.Id);
        }

        public Task<AppUser> GetUserByMailAsync(string email)
        {
            return this._userManager.FindByEmailAsync(email);
        }

        public Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return this._userManager.FindByNameAsync(username);
        }

        public Task<AppUser> GetUserByIdAsync(Guid userId)
        {
            return this._userManager.FindByIdAsync(userId.ToString());
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
