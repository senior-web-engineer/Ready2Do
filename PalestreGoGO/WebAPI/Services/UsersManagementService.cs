using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
using Serilog;
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
        private readonly IRichiesteRegistrazioneRepository _richiesteRegistrazioniRepository;
        private readonly B2CGraphClient _b2cClient;
        private readonly IConfiguration _config;

        private static Random s_random = new Random(DateTime.Now.Millisecond);

        public UsersManagementService(/*UserManager<AppUser> userManager,*/
                                      B2CGraphClient b2cClient,
                                      IConfiguration config,
                                      IUserConfirmationService confirmService,
                                      ILogger<UsersManagementService> logger,
                                      IClientiProvisioner clientiProvisioner,
                                      IClientiRepository repository,
                                      IUtentiRepository utentiRepository,
                                      IClientiUtentiRepository repositoryClientiUtenti,
                                      IRichiesteRegistrazioneRepository richiesteRegistrazioniRepository)
        {
            _logger = logger;
            _config = config;
            _b2cClient = b2cClient;
            _confirmUserService = confirmService;
            _clientiProvisioner = clientiProvisioner;
            _repository = repository;
            _utentiRepository = utentiRepository;
            _repositoryClientiUtenti = repositoryClientiUtenti;
            _richiesteRegistrazioniRepository = richiesteRegistrazioniRepository;
        }

        public async Task SendConfirmationEmailAsync(string userEmail, Guid? correlationId = null, DateTime? expirationDate = null, string nome = null, string cognome = null)
        {
            DateTime expiration = DateTime.Now.AddMinutes(_config.GetValue<int>("Provisioning:ValidationEmailValidityMinutes", Constants.DEFAULT_VALIDATION_VALIDITY));
            var code = await _richiesteRegistrazioniRepository.NuovaRichiestaRegistrazioneAsync(userEmail, expirationDate, correlationId, null);

            var email = new ConfirmationMailMessage(userEmail, code,                                                    
                                                    _config.GetValue<string>("Provisioning:EmailConfirmationUrl"),
                                                    true,
                                                    cognome:cognome,
                                                    nome: nome);
            await _confirmUserService.EnqueueConfirmationMailRequestAsync(email);
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
            EsitoConfermaRegistrazioneDM esitoConferma = null;
            try
            {
                esitoConferma = await _richiesteRegistrazioniRepository.CompletaRichiestaRegistrazioneAsync(username, code);
                var result = new UserConfirmationResultAPIModel(user.Id)
                {
                    IdStrutturaAffiliate = esitoConferma.IdRefereer,
                    IdCliente = esitoConferma.IdCliente
                };
                return result;
            }
            catch (UserConfirmationException)
            {
                _logger.LogWarning($"ConfirmUserEmailAsync -> Failed validation for user: {username} with code: [{code}]");
                return new UserConfirmationResultAPIModel()
                {
                    IdUser = user.Id
                };
            }
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
        public async Task<AzureUser> GetOrCreateUserAsync(AzureUser newUser)
        {
            if ((newUser == null) || (newUser.SignInNames == null) || (newUser.SignInNames.Count == 0))
            {
                throw new ArgumentException(nameof(newUser));
            }
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
        public async Task AggiungiStrutturaOwnedAsync(AzureUser user, int idCliente)
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

        public async Task TryDeleteStrutturaOwnedAsync(string userId, int idCliente)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException(nameof(userId));
            var user = await GetUserByIdAsync(userId);
            //Se non ha strutture associate ritorniamo
            if (string.IsNullOrWhiteSpace(user.StruttureOwned))
            {
                Log.Warning("Nessuna StrutturaOwned per l'utente: [{userId}] - Impossibile rimuovere la StrutturaGestita [{idCliente}]", userId, idCliente);
                return;
            }
            string[] struttureOwned = user.StruttureOwned
                                                .Split(',')
                                                .Where(s => !s.Trim().Equals(idCliente.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                                .Select(s => s.Trim())
                                                .ToArray();
            await _b2cClient.UpdateUserStruttureOwnedAsync(userId, string.Join(',', struttureOwned));
        }

        public async Task SaveProfileChangesAsync(string userId, UtenteInputDM profilo)
        {
            Log.Debug("Salvataggio profilo: {@Profilo}", profilo);
            var azUser = new AzureUser()
            {
                Cognome = profilo.Cognome,
                DisplayName = profilo.DisplayName,
                Nome = profilo.Nome,
                TelephoneNumber = profilo.TelephoneNumber
            };
            await _b2cClient.UpdateUserProfile(userId, azUser);
        }
    }
}
