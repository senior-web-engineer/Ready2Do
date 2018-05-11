using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Model;
using PalestreGoGo.WebAPI.ViewModel;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserConfirmationService _confirmUserService;
        private readonly ILogger<UsersManagementService> _logger;
        private readonly IClientiProvisioner _clientiProvisioner;
        private readonly IClientiRepository _repository;

        public UsersManagementService(UserManager<AppUser> userManager,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner,
                                      IClientiRepository repository
                                      )
        {
            this._logger = logger;
            this._userManager = userManager;
            this._confirmUserService = confirmService;
            this._clientiProvisioner = clientiProvisioner;
            this._repository = repository;
        }

        public async Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return new UserConfirmationViewModel(false);
            var esitoConferma = await _userManager.ConfirmEmailAsync(user, code);
            if (!esitoConferma.Succeeded)
            {
                _logger.LogWarning($"ConfirmUserAsync -> Failed validation for user: {username} with code: [{code}]");
                return new UserConfirmationViewModel()
                {
                    IdUser = user.Id
                };
            }
            var result = new UserConfirmationViewModel(user.Id);
            var claims = await _userManager.GetClaimsAsync(user);
            var claimStructureOwned = claims.SingleOrDefault(c => c.Type.Equals(Constants.ClaimStructureOwned));
            //Se è un owner ==> facciamo il provisioning del cliente
            if (claimStructureOwned != null)
            {
                await _clientiProvisioner.ProvisionClienteAsync(user.CreationToken, user.Id);
                result.IdStrutturaAffiliate = int.Parse(claimStructureOwned.Value);
            }
            else
            {
                //Se è un utente "ORDINARIO" ed in fase di registrazione è stata specificata la struttura di affiliazione andiamo a censire l'associazione sul DB
                var claimSrutturaAffiliata = claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureAffiliated));
                if (claimSrutturaAffiliata != null)
                {
                    int idStrutturaAffiliata = int.Parse(claimSrutturaAffiliata.Value);
                    await _repository.AddUtenteFollowerAsync(idStrutturaAffiliata, user.Id);
                    result.IdStrutturaAffiliate = idStrutturaAffiliata;
                }

            }
            _logger.LogInformation($"ConfirmUserAsync -> Successfully validated user {username}");
            return result;
        }

        public Task<AppUser> GetUserByMailAsync(string email)
        {
            return this._userManager.FindByEmailAsync(email);
        }

        public Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return this._userManager.FindByNameAsync(username);
        }

        public async Task<IList<Claim>> GetUserCalimsAsync(AppUser user)
        {
            if (user == null) return null;
            return await _userManager.GetClaimsAsync(user);
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

            var newUser = await internalCreateUserAsync(user, password, true);

            // Rendiamo l'utente OWNER della struttura (associando il claim all'utente)
            await _userManager.AddClaimAsync(newUser, new Claim(Constants.ClaimStructureOwned, idCliente));
            _logger.LogInformation($"Created a new owner. UserId: {user.Id}");

            return user.Id;
        }

        public async Task<Guid> RegisterUserAsync(AppUser user, string password, int? idStrutturaAffiliata)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentNullException(nameof(user.UserName));
            if (string.IsNullOrWhiteSpace(user.FirstName)) throw new ArgumentNullException(nameof(user.FirstName));
            if (string.IsNullOrWhiteSpace(user.LastName)) throw new ArgumentNullException(nameof(user.LastName));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            var newUser = await internalCreateUserAsync(user, password, false);

            // Creiamo il claim con la struttura da cui arriva l'utente (Affiliato principale)
            if (idStrutturaAffiliata.HasValue)
            {
                await _userManager.AddClaimAsync(user, new Claim(Constants.ClaimStructureAffiliated, idStrutturaAffiliata.Value.ToString()));
            }
            _logger.LogInformation($"Created a new user (UserId: {user.Id}) affiliated with referer: {idStrutturaAffiliata}");

            return user.Id;
        }

        protected async Task<AppUser> internalCreateUserAsync(AppUser user, string password, bool isCliente)
        {
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
            await _confirmUserService.SendConfirmationMailRequestAsync(new ConfirmationMailMessage(user.UserName, code, user.CreationToken, isCliente));
            _logger.LogInformation($"Created a new account with password and enqueued confirmation mail send. UserId: {user.Id}");
            //Rileggiamo l'utente appena creato 
            return await _userManager.FindByNameAsync(user.UserName);
        }
    }
}
