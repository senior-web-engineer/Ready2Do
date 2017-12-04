using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Palestregogo.STS.Model.Identity;
using Microsoft.AspNetCore.Identity;
using Palestregogo.STS.Business;
using Microsoft.EntityFrameworkCore;
using Palestregogo.STS.Services;

namespace Palestregogo.STS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<STSIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, AppRole>(cfg =>
            {
                cfg.Password.RequiredLength = 8;
                cfg.Password.RequireLowercase = true;
                cfg.Password.RequireUppercase = true;
                cfg.Password.RequireDigit = true;
                cfg.Password.RequireNonAlphanumeric = true;

                //Richiediamo la conferma dell'email
                // Per ora disabilitato, dobbiamo implementare il servizio EmailSender prima di abilitarla
                cfg.SignIn.RequireConfirmedEmail = true;

                cfg.User.RequireUniqueEmail = true; //In caso di utente con registrazione locale e Google?
            })
                .AddEntityFrameworkStores<STSIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.

            services.AddMvc();

            // configure identity server with in-memory stores, keys, clients and scopes
            STSConfig.Configuration = Configuration;
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(STSConfig.GetIdentityResources())
                .AddInMemoryApiResources(STSConfig.GetApiResources())
                .AddInMemoryClients(STSConfig.GetClients())
                .AddAspNetIdentity<AppUser>();

            services.AddAuthentication();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("api", "api/{controller}/{id?}");
            });
        }
    }
}
