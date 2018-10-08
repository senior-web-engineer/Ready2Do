using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Model;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class UsersManagementService : IUsersManagementService
    {
        //private readonly UserManager<AppUser> _userManager;
        private readonly IUserConfirmationService _confirmUserService;
        private readonly ILogger<UsersManagementService> _logger;
        private readonly IClientiProvisioner _clientiProvisioner;
        private readonly IClientiRepository _repository;
        private readonly B2CGraphClient _b2cClient;
        public UsersManagementService(/*UserManager<AppUser> userManager,*/
                                      B2CGraphClient b2cClient,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner,
                                      IClientiRepository repository
                                      )
        {
            this._logger = logger;
            //this._userManager = userManager;
            this._b2cClient = b2cClient;
            this._confirmUserService = confirmService;
            this._clientiProvisioner = clientiProvisioner;
            this._repository = repository;
        }

        public async Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code)
        {
            throw new NotImplementedException();
            //var user = await _userManager.FindByNameAsync(username);
            //if (user == null) return new UserConfirmationViewModel(false);
            //var esitoConferma = await _userManager.ConfirmEmailAsync(user, code);
            //if (!esitoConferma.Succeeded)
            //{
            //    _logger.LogWarning($"ConfirmUserAsync -> Failed validation for user: {username} with code: [{code}]");
            //    return new UserConfirmationViewModel()
            //    {
            //        IdUser = user.Id
            //    };
            //}
            //var result = new UserConfirmationViewModel(user.Id);
            //var claims = await _userManager.GetClaimsAsync(user);
            //var claimStructureOwned = claims.SingleOrDefault(c => c.Type.Equals(Constants.ClaimStructureOwned));
            ////Se è un owner ==> facciamo il provisioning del cliente
            //if (claimStructureOwned != null)
            //{
            //    await _clientiProvisioner.ProvisionClienteAsync(user.CreationToken, user.Id);
            //    result.IdStrutturaAffiliate = int.Parse(claimStructureOwned.Value);
            //}
            //else
            //{
            //    //Se è un utente "ORDINARIO" ed in fase di registrazione è stata specificata la struttura di affiliazione andiamo a censire l'associazione sul DB
            //    var claimSrutturaAffiliata = claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureAffiliated));
            //    if (claimSrutturaAffiliata != null)
            //    {
            //        int idStrutturaAffiliata = int.Parse(claimSrutturaAffiliata.Value);
            //        await _repository.AddUtenteFollowerAsync(idStrutturaAffiliata, user.Id);
            //        result.IdStrutturaAffiliate = idStrutturaAffiliata;
            //    }

            //}
            //_logger.LogInformation($"ConfirmUserAsync -> Successfully validated user {username}");
            //return result;
        }

        public async Task<LocalAccountUser> GetUserByMailAsync(string email)
        {            
            return (await this._b2cClient.GetUserByMailAsync(email))?.FirstOrDefault();
        }

        //public Task<LocalAccountUser> GetUserByUsernameAsync(string username)
        //{
        //    return (await this._b2cClient.GetUserByMailAsync(email))?.FirstOrDefault();
        //    throw new NotImplementedException();
        //    //return this._userManager.FindByNameAsync(username);
        //}

        public async Task<IList<Claim>> GetUserCalimsAsync(LocalAccountUser user)
        {
            if (user == null) return null;
            throw new NotImplementedException();
            //return await _userManager.GetClaimsAsync(user);
        }


        public async Task<LocalAccountUser> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
            //return this._userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<Guid> RegisterOwnerAsync(LocalAccountUser user, string idCliente)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(idCliente)) throw new ArgumentNullException(nameof(idCliente));

            var createdUser = await _b2cClient.CreateUserAsync(user);

            // Rendiamo l'utente OWNER della struttura (associando il claim all'utente)
            throw new NotImplementedException();
            //await _userManager.AddClaimAsync(newUser, new Claim(Constants.ClaimStructureOwned, idCliente));
            //_logger.LogInformation($"Created a new owner. UserId: {user.Id}");

            //return user.Id;
        }

        public async Task<Guid> RegisterUserAsync(LocalAccountUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var createdUser = await _b2cClient.CreateUserAsync(user);
            _logger.LogInformation($"Created a new user (UserId: {createdUser.UserPrincipalName}) affiliated with referer: {createdUser.Refereer}");

            return createdUser.Id.Value;
        }

        protected async Task<AppUser> internalCreateUserAsync(LocalAccountUser user, string password, bool isCliente)
        {
            //var result = await _userManager.CreateAsync(user, password);
            //if (!result.Succeeded)
            //{
            //    var errors = result.Errors.Select(e => e.Description).Aggregate((i, j) => $"{i} | {j} ");
            //    throw new ApplicationException($"Errore durante il salvataggio dell'utente. Errors: {errors}");
            //}
            //_logger.LogDebug($"User with email: {user.Email} persisted");
            //// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            //// Send an email with this link
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //_logger.LogTrace($"RegisterUser-> generated code [{code}]for user: {user.UserName}");
            //await _confirmUserService.SendConfirmationMailRequestAsync(new ConfirmationMailMessage(user.UserName, code, user.CreationToken, isCliente));
            //_logger.LogInformation($"Created a new account with password and enqueued confirmation mail send. UserId: {user.Id}");
            ////Rileggiamo l'utente appena creato 
            //return await _userManager.FindByNameAsync(user.UserName);
            throw new NotImplementedException();
            //
        }

        //protected async Task<string> GenerateEmailConfirmationTokenAsync(LocalAccountUser user)
        //{
        //    //TODO: Generare il token in modo sicuro
            
        //}
    }
}
