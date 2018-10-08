using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.DataModel.Exceptions;
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
        private readonly IUtentiRepository _utentiRepository;
        private readonly B2CGraphClient _b2cClient;

        private static Random s_random = new Random(DateTime.Now.Millisecond);

        public UsersManagementService(/*UserManager<AppUser> userManager,*/
                                      B2CGraphClient b2cClient,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner,
                                      IClientiRepository repository,
                                      IUtentiRepository utentiRepository
                                      )
        {
            this._logger = logger;
            //this._userManager = userManager;
            this._b2cClient = b2cClient;
            this._confirmUserService = confirmService;
            this._clientiProvisioner = clientiProvisioner;
            this._repository = repository;
            this._utentiRepository = utentiRepository;
        }

        public async Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code)
        {

            var user = await _b2cClient.GetUserByMailAsync(username);
            //if (user == null) return new UserConfirmationViewModel(false);
            RichiestaRegistrazione richiesta = null;
            try
            {                
                richiesta = await _utentiRepository.CompletaRichiestaRegistrazioneAsync(username, code);
            }
            catch(UserConfirmationException)
            {
                _logger.LogWarning($"ConfirmUserAsync -> Failed validation for user: {username} with code: [{code}]");
                return new UserConfirmationViewModel()
                {
                    IdUser = user.Id.Value
                };
            }
            var result = new UserConfirmationViewModel(user.Id.Value);
            var claimStructureOwned = user.StruttureOwned;
            //Se è un owner ==> facciamo il provisioning del cliente
            if (!string.IsNullOrWhiteSpace(claimStructureOwned))
            { 
                await _clientiProvisioner.ProvisionClienteAsync(richiesta.CorrelationId.ToString(), user.Id.Value);
                result.IdStrutturaAffiliate = int.Parse(claimStructureOwned);
            }
            else
            {
                //Se è un utente "ORDINARIO" ed in fase di registrazione è stata specificata la struttura di affiliazione andiamo a censire l'associazione sul DB
                if (!string.IsNullOrWhiteSpace(user.Refereer))
                {
                    int idStrutturaAffiliata = int.Parse(user.Refereer);
                    await _repository.AddUtenteFollowerAsync(idStrutturaAffiliata, user.Id.Value);
                    result.IdStrutturaAffiliate = idStrutturaAffiliata;
                }

            }
            _logger.LogInformation($"ConfirmUserAsync -> Successfully validated user {username}");
            return result;
        }

        public async Task<LocalAccountUser> GetUserByMailAsync(string email)
        {
            return await this._b2cClient.GetUserByMailAsync(email);
        }

        //public Task<LocalAccountUser> GetUserByUsernameAsync(string username)
        //{
        //    return (await this._b2cClient.GetUserByMailAsync(email))?.FirstOrDefault();
        //    throw new NotImplementedException();
        //    //return this._userManager.FindByNameAsync(username);
        //}

        //public async Task<IList<Claim>> GetUserCalimsAsync(LocalAccountUser user)
        //{
        //    if (user == null) return null;
        //    throw new NotImplementedException();
        //    //return await _userManager.GetClaimsAsync(user);
        //}


        public async Task<LocalAccountUser> GetUserByIdAsync(string userId)
        {
            return await _b2cClient.GetUserById(userId);
        }

        public async Task<Guid> RegisterOwnerAsync(LocalAccountUser user, string idCliente, Guid correlationId)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(idCliente)) throw new ArgumentNullException(nameof(idCliente));
            if (!string.IsNullOrWhiteSpace(user.StruttureOwned)) throw new ArgumentException("L'utente è già owner di una struttura");
            // ATTENZIONE! Eventuali altre strutture vengono sovrascritte
            user.StruttureOwned = idCliente.ToString(); 
            //var createdUser = await _b2cClient.CreateUserAsync(user);
            var createdUser = await internalCreateUserAsync(user, false);
            _logger.LogInformation($"Created a new owner. UserId: {createdUser.Id}");
            return createdUser.Id.Value;
        }

        public async Task<Guid> RegisterUserAsync(LocalAccountUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            //var createdUser = await _b2cClient.CreateUserAsync(user);
            var createdUser = await internalCreateUserAsync(user, false);
            _logger.LogInformation($"Created a new user (UserId: {createdUser.UserPrincipalName}) affiliated with referer: {createdUser.Refereer}");

            return createdUser.Id.Value;
        }

        protected async Task<LocalAccountUser> internalCreateUserAsync(LocalAccountUser user, bool isCliente, Guid? correlationId = null)
        {
            var createdUser = await _b2cClient.CreateUserAsync(user);
            _logger.LogDebug($"User with email: {user.Emails[0]} created");
            string code = GenerateEmailConfirmationToken(createdUser);
            string userName = user.SignInNames[0].Value;
            var richiesta = await _utentiRepository.RichiestaRegistrazioneSalvaAsync(userName, code, correlationId);
            _logger.LogTrace($"RegisterUser-> generated code [{code}]for user: {userName}");
            await _confirmUserService.SendConfirmationMailRequestAsync(new ConfirmationMailMessage(userName, code, richiesta.CorrelationId.ToString("D"), isCliente));
            _logger.LogInformation($"Created a new account with password and enqueued confirmation mail send. UserId: {createdUser.Id}");
            return createdUser;
        }

        protected string GenerateEmailConfirmationToken(LocalAccountUser user)
        {
            //TODO: Generare il token in modo sicuro
            return s_random.Next(1000000, 9999999).ToString();
        }

    }
}
