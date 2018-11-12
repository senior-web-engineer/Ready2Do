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
    public interface IUsersManagementService
    {
        Task<string> RegisterOwnerAsync(AzureUser user, string idCliente, Guid correlationId);

        Task<string> RegisterUserAsync(AzureUser user);

        Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code);

        Task<AzureUser> GetUserByMailAsync(string email);

        //Task<LocalAccountUser> GetUserByUsernameAsync(string username);

        Task<AzureUser> GetUserByIdAsync(string id);

        //Task<IList<Claim>> GetUserCalimsAsync(LocalAccountUser user);
    }
}
