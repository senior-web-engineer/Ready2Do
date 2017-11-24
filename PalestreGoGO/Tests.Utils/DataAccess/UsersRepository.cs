using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PalestreGoGo.IdentityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Tests.Utils.DataAccess
{
    public static class UsersRepository
    {
        private static IDbConnection GetConnection() => new SqlConnection(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);

        internal static string ConnectionString { get => ConfigurationManager.ConnectionStrings["Default"].ConnectionString; }


        //public static async Task<Guid> AddUser(string userName, string password, string token, bool confirmed = false)
        //{
        //    AppUser appUser = null;
        //    try
        //    {
        //        DbContextOptionsBuilder optBuilder = new DbContextOptionsBuilder();
        //        optBuilder.UseSqlServer(UsersRepository.ConnectionString);
        //        var dbContext = new AppIdentityDbContext(optBuilder.Options);
        //        appUser = new AppUser(userName, userName, userName, userName, token);
        //        UserManager<AppUser> userManager = new UserManager<AppUser>(new UserStore<AppUser, AppRole, AppIdentityDbContext, Guid>(dbContext), null, null, null, null, null, null, null, null);
        //        var result = await userManager.CreateAsync(appUser);
        //        if (!result.Succeeded) throw new ApplicationException(result.Errors.First().Description);
        //        await userManager.UpdateSecurityStampAsync(appUser);
        //        appUser.NormalizedUserName = appUser.NormalizedEmail = appUser.UserName = appUser.Email;
        //        await userManager.UpdateNormalizedUserNameAsync(appUser);
        //        await userManager.UpdateNormalizedEmailAsync(appUser);
        //        await userManager.UpdateAsync(appUser);
        //        if (confirmed)
        //        {
        //            var code = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
        //            await userManager.ConfirmEmailAsync(appUser, code);
        //        }
        //        appUser = await userManager.FindByEmailAsync(userName);

        //    }
        //    catch (Exception exc)
        //    {
        //        Console.WriteLine(exc.ToString());
        //    }
        //    return appUser.Id;
        //}

    }
}
