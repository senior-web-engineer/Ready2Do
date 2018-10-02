using AutoMapper;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataAccess.Interfaces;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel.Mappers;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        protected void SetupRoles()
        {

        }

        private void RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient<IAbbonamentiRepository, AbbonamentiRepository>();
            services.AddTransient<IAppuntamentiRepository, AppuntamentiRepository>();
            services.AddTransient<IClientiRepository, ClientiRepository>();
            services.AddTransient<ILocationsRepository, LocationsRepository>();
            services.AddTransient<ISchedulesRepository, SchedulesRepository>();
            services.AddTransient<ITipologieAbbonamentiRepository, TipologieAbbonamentiRepository>();
            services.AddTransient<ITipologieClientiRepository, TipologieClientiRepository>();
            services.AddTransient<ITipologieLezioniRepository, TipologieLezioniRepository>();
            services.AddTransient<IMailTemplatesRepository, MailTemplatesRepository>();
            services.AddTransient<IUtentiRepository, UtentiRepository>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddDataAccessRepositories(opt =>
            {
                opt.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            });

            services.AddTransient<IClientiProvisioner, ClientiProvisioner>();
            services.AddTransient<IUserConfirmationService, UserConfirmationService>();
            services.AddTransient<IUsersManagementService, UsersManagementService>();

            RegisterRepositories(services);
            
            services.AddMvc()
                .AddFluentValidation();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["STS:Issuer"];
                    options.ApiName = "palestregogo.api";
                    options.RequireHttpsMetadata = false; //Disabilitare per la produzione
                    options.EnableCaching = true;
                    options.CacheDuration = new System.TimeSpan(0, 5, 0); //5 minuti cache
                });

            services.AddIdentity<AppUser, AppRole>(cfg =>
            {
                cfg.Password.RequiredLength = 4;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequireNonAlphanumeric = false;

                //Richiediamo la conferma dell'email
                // Per ora disabilitato, dobbiamo implementare il servizio EmailSender prima di abilitarla
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true; //In caso di utente con registrazione locale e Google?

            })
             .AddEntityFrameworkStores<AppIdentityDbContext>()
             .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("tipologiche.edit",
                //    policy => policy.AddRequirements(new HasScopeRequirement(Constants.ScopeTipologicheEdit, Configuration["STS:Issuer"])));
                options.AddPolicy("UsersManagement",
                    policy => policy.AddRequirements(new HasScopeRequirement(Constants.ClaimStructureManaged, Configuration["STS:Issuer"])));
                //options.AddPolicy("ProvisioningPolicy",
                //    policy => policy.AddRequirements(new HasScopeRequirement(Constants.ScopeProvisioningClienti, Configuration["STS:Issuer"])));
            });

            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToViewModelMappingProfile>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Disable Application Insights
            //var config = app.ApplicationServices.GetService<TelemetryConfiguration>();
            //if (config != null) config.DisableTelemetry = true;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.SetupUsersAndRoles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseCors()
            app.UseMvc();

        }
    }
}
