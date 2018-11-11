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
        Task<string> RegisterOwnerAsync(LocalAccountUser user, string idCliente, Guid correlationId);

        Task<string> RegisterUserAsync(LocalAccountUser user);

        Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code);

        Task<LocalAccountUser> GetUserByMailAsync(string email);

        //Task<LocalAccountUser> GetUserByUsernameAsync(string username);

        Task<LocalAccountUser> GetUserByIdAsync(string id);

        //Task<IList<Claim>> GetUserCalimsAsync(LocalAccountUser user);
    }
}
