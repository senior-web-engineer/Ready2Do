using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.DataModel.Exceptions;
using PalestreGoGo.WebAPI.Model;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly IUserConfirmationService _confirmUserService;
        private readonly ILogger<UsersManagementService> _logger;
        private readonly IClientiProvisioner _clientiProvisioner;
        private readonly IClientiRepository _repository;
        private readonly IClientiUtentiRepository _repositoryClientiUtenti;
        private readonly IUtentiRepository _utentiRepository;
        private readonly B2CGraphClient _b2cClient;

        private static Random s_random = new Random(DateTime.Now.Millisecond);

        public UsersManagementService(/*UserManager<AppUser> userManager,*/
                                      B2CGraphClient b2cClient,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner,
                                      IClientiRepository repository,
                                      IUtentiRepository utentiRepository,
                                      IClientiUtentiRepository repositoryClientiUtenti
                                      )
        {
            _logger = logger;
            _b2cClient = b2cClient;
            _confirmUserService = confirmService;
            _clientiProvisioner = clientiProvisioner;
            _repository = repository;
            _utentiRepository = utentiRepository;
            _repositoryClientiUtenti = repositoryClientiUtenti;
        }

        /// <summary>
        /// Conferma l'email di un Utente tramite il codice univoco precedentemente inviatogli all'indirizzo indicato 
        /// in fase di registrazione
        /// </summary>
        /// <param name="username"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<UserConfirmationResultAPIModel> ConfirmUserEmailAsync(string username, string code)
        {

            var user = await _b2cClient.GetUserByMailAsync(username);
            _logger.LogWarning($"ConfirmUserEmailAsync -> User [{username}]not founded");
            if (user == null) return new UserConfirmationResultAPIModel(false);
            EsitoConfermaRegistrazioneDM esitoConferma= null;
            try
            {
                esitoConferma = await _utentiRepository.CompletaRichiestaRegistrazioneAsync(username, code);
            }
            catch (UserConfirmationException)
            {
                _logger.LogWarning($"ConfirmUserEmailAsync -> Failed validation for user: {username} with code: [{code}]");
                return new UserConfirmationResultAPIModel()
                {
                    IdUser = user.Id
                };
            }


            var result = new UserConfirmationResultAPIModel(user.Id);
            var claimStructureOwned = user.StruttureOwned;

            //Se è un owner ==> facciamo il provisioning del cliente
            if (!string.IsNullOrWhiteSpace(claimStructureOwned))
            {
                await _clientiProvisioner.ProvisionClienteAsync(richiesta.CorrelationId.ToString(), user.Id);
                result.IdStrutturaAffiliate = int.Parse(claimStructureOwned);
            }
            else
            {
                //Se è un utente "ORDINARIO" ed in fase di registrazione è stata specificata la struttura di affiliazione andiamo a censire l'associazione sul DB
                if (!string.IsNullOrWhiteSpace(user.Refereer))
                {
                    int idStrutturaAffiliata = int.Parse(user.Refereer);
                    await _repositoryClientiUtenti.AssociaUtenteAsync(idStrutturaAffiliata, user.Id, user.Nome, user.Cognome, user.DisplayName);
                    result.IdStrutturaAffiliate = idStrutturaAffiliata;
                }

            }
            _logger.LogInformation($"ConfirmUserAsync -> Successfully validated user {username}");
            return result;
        }

        public async Task<AzureUser> GetUserByMailAsync(string email)
        {
            return await _b2cClient.GetUserByMailAsync(email);
        }

        public async Task<AzureUser> GetUserByIdAsync(string userId)
        {
            return await _b2cClient.GetUserById(userId);
        }

        /// <summary>
        /// Crea un nuovo utente
        /// </summary>
        /// <param name="newUser">Dati del nuovo utente</param>
        /// <returns>Ritorna l'utente creato</returns>
        public async Task<AzureUser> CreateUserAsync(AzureUser newUser)
        {
            return await _b2cClient.CreateUserAsync(newUser);
        }

        /// <summary>
        /// Crea un nuovo utente
        /// </summary>
        /// <param name="newUser">Dati del nuovo utente</param>
        /// <returns>Ritorna l'utente creato</returns>
        public async Task<AzureUser> GetOrCreateUserAsyn(AzureUser newUser)
        {
            //Se l'utente esiste già lo recuperiamo
            var user = await _b2cClient.GetUserByMailAsync(newUser.SignInNames[0].Value);
            if (user != null)
            {
                return user;
            }
            //Altrimenti lo creiamo
            else
            {
                return await _b2cClient.CreateUserAsync(newUser);
            }
        }

        //Aggiunge all'utente passato l'idCliente come struttura gestita
        public async Task AggiungiStrutturaGestitaAsync(AzureUser user, int idCliente)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (!string.IsNullOrWhiteSpace(user.StruttureOwned))
            {
                var strutture = user.StruttureOwned.Split(',');
                if (strutture.Contains(idCliente.ToString()))
                {
                    //Se già presente non facciamo niente
                    return;
                }
                else
                {
                    user.StruttureOwned = string.Format("{0},{1}", user.StruttureOwned, idCliente);
                }
            }
            else
            {
                user.StruttureOwned = idCliente.ToString();
            }
            await _b2cClient.UpdateUserStruttureOwnedAsync(user.Id, user.StruttureOwned);
        }

        //public async Task<string> RegisterOwnerAsync(AzureUser user, string idCliente, Guid correlationId)
        //{
        //    if (user == null) throw new ArgumentNullException(nameof(user));
        //    if (string.IsNullOrWhiteSpace(idCliente)) throw new ArgumentNullException(nameof(idCliente));
        //    if (!string.IsNullOrWhiteSpace(user.StruttureOwned)) throw new ArgumentException("L'utente è già owner di una struttura");
        //    // ATTENZIONE! Eventuali altre strutture vengono sovrascritte
        //    user.StruttureOwned = idCliente.ToString(); 
        //    //var createdUser = await _b2cClient.CreateUserAsync(user);
        //    var createdUser = await internalCreateUserAsync(user, false);
        //    _logger.LogInformation($"Created a new owner. UserId: {createdUser.Id}");
        //    return createdUser.Id;
        //}

        public async Task<string> RegisterUserAsync(AzureUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            //var createdUser = await _b2cClient.CreateUserAsync(user);
            var createdUser = await internalCreateUserAsync(user, false);
            _logger.LogInformation($"Created a new user (UserId: {createdUser.UserPrincipalName}) affiliated with referer: {createdUser.Refereer}");

            return createdUser.Id;
        }

        protected async Task<AzureUser> internalCreateUserAsync(AzureUser user, bool isCliente, Guid? correlationId = null)
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

        protected string GenerateEmailConfirmationToken(AzureUser user)
        {
            //TODO: Generare il token in modo sicuro
            return s_random.Next(1000000, 9999999).ToString();
        }

    }
}
