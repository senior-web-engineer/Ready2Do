using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Utils;
using System.Collections.Generic;
using System.Security.Claims;

namespace Tests.WebAPI
{
    public static class Utils
    {

        public static readonly int ID_CLIENTE_TEST_1 = 1;
        public static readonly int ID_CLIENTE_TEST_2 = 2;

        public static readonly int ID_CLIENTE_TEST_1_TIPO_LEZIONE1 = 1;
        public static readonly int ID_CLIENTE_TEST_1_TIPO_LEZIONE2 = 2;
        public static readonly int ID_CLIENTE_TEST_2_TIPO_LEZIONE1 = 3;

        public static readonly IConfigurationRoot Config;

        static Utils()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public static PalestreGoGoDbContext BuildDbContext()
        {
            var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkSqlServer()
                        .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<PalestreGoGoDbContext>();

            builder.UseSqlServer(Config.GetConnectionString("DefaultConnection"))
                    .UseInternalServiceProvider(serviceProvider);

            return new PalestreGoGoDbContext(builder.Options);
        }

        public static ClaimsPrincipal GetGlobalAdminUser()
        {
            var claims = new List<Claim>()
            {
                new Claim(PalestreGoGo.WebAPI.Constants.ClaimRole, PalestreGoGo.WebAPI.Constants.RoleGlobalAdmin)
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TESTAUTH"));
        }

        public static ClaimsPrincipal GetAnonymousUser() { 
            return new ClaimsPrincipal();
        }
    }
}
