using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using System.Text;
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

        private void RegisterService(IServiceCollection services)
        {
            services.AddTransient<B2CGraphClient, B2CGraphClient>();
            services.AddTransient<IClientiProvisioner, ClientiProvisioner>();
            services.AddTransient<IUserConfirmationService, UserConfirmationService>();
            services.AddTransient<IUsersManagementService, UsersManagementService>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<GraphAPIOptions>(Configuration.GetSection("Authentication:GraphAPI"));

            ApiConfigs.InitConfiguration(Configuration);

            services.AddDataAccessRepositories(opt =>
            {
                opt.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            });

            RegisterService(services);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //Aggiungiamo due2 dsitinti JWTBeare per gestire le 2 authority (user/struttura)
                //.AddJwtBearer(jwtOptions =>
                //{
                //    jwtOptions.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["Authentication:AzureAdB2C:Tenant"]}/{Configuration["Authentication:AzureAdB2C:UserPolicy"]}/v2.0/";
                //    jwtOptions.Audience = Configuration["Authentication:AzureAdB2C:ClientId"];
                //    jwtOptions.Events = new JwtBearerEvents
                //    {
                //        OnAuthenticationFailed = AuthenticationFailed,
                //        OnMessageReceived = 
                //    };
                //})
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["Authentication:AzureAdB2C:Tenant"]}/{Configuration["Authentication:AzureAdB2C:StrutturaPolicy"]}/v2.0/";
                    jwtOptions.Audience = Configuration["Authentication:AzureAdB2C:ClientId"];
                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = AuthenticationFailed,
                        OnMessageReceived = MessageReceived
                    };
                });
           
            services.AddMvc()
                // Abilitiamo i comportamenti introdotti con ASP.NET Core 2.2 (più recenti)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                // Abilitiamo la FLUENTVALIDATION 
                .AddFluentValidation(fv=> {
                    //Registriamo tutti i validators (le classi che estendono AbstractValidator) presente nell'assembly che contiene Startup (il corrente)
                    fv.RegisterValidatorsFromAssemblyContaining(typeof(Startup));
                    //Disabilitiamo il validator di default
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            //app.SetupUsersAndRoles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseCors()
            app.UseMvc();

        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }

        private Task MessageReceived(MessageReceivedContext context)
        {
            return Task.FromResult(0);
        }
    }
}
