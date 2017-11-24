using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PalestreGoGo.IdentityModel;

namespace PalestreGoGo.WebAPI.Utils
{
    public static class IdentityExtensions
    {
        public static void SetupUsersAndRoles(this IApplicationBuilder app)
        {
            IServiceScopeFactory scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                RoleManager<AppRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                // Creiamo il ruolo di GlobalAdmin se non esiste
                AppRole globalAdmin = roleManager.FindByNameAsync(Constants.RoleGlobalAdmin).Result;
                if (globalAdmin == null)
                {
                    globalAdmin = new AppRole()
                    {
                        Name = Constants.RoleGlobalAdmin,
                        NormalizedName = roleManager.NormalizeKey(Constants.RoleGlobalAdmin),
                        ConcurrencyStamp = Guid.NewGuid().ToString("N")
                    };
                    roleManager.CreateAsync(globalAdmin).Wait();
                }
            }
        }
    }
}
