using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public interface IUsersManagementService
    {
        Task<Guid> RegisterOwnerAsync(AppUser user, string password, string idCliente);

        Task<UserConfirmationViewModel> ConfirmUserAsync(string username, string code);

        Task<AppUser> GetUserByMailAsync(string email);

        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<AppUser> GetUserByIdAsync(Guid id);
    }
}
