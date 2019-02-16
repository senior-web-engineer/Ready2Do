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
    public interface IUsersManagementService
    {
        Task<AzureUser> GetOrCreateUserAsync(AzureUser newUser);

        Task AggiungiStrutturaOwnedAsync(AzureUser user, int idCliente);
        Task TryDeleteStrutturaOwnedAsync(string userId, int idCliente);
        Task<AzureUser> GetUserByIdAsync(string id);

        Task<UserConfirmationResultAPIModel> ConfirmUserEmailAsync(string username, string code);

        Task<AzureUser> GetUserByMailAsync(string email);

        Task SaveProfileChangesAsync(string userId, UtenteInputDM profilo);

        Task SendConfirmationEmailAsync(string userEmail, Guid? correlationId = null, DateTime? expirationDate = null, string nome = null, string cognome = null);

        //        Task<string> RegisterUserAsync(AzureUser user);

        //Task<LocalAccountUser> GetUserByUsernameAsync(string username);


        //Task<IList<Claim>> GetUserCalimsAsync(LocalAccountUser user);
    }
}
