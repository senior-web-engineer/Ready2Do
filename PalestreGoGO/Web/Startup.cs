﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Web.Services;
using Web.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;
using Web.Utils;
using Microsoft.AspNetCore.Authentication;


//using 
namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));
            services.Configure<GraphAPIOptions>(Configuration.GetSection("Authentication:GraphAPI"));

            // Add application services.
            services.AddTransient<AccountServices, AccountServices>();
            services.AddTransient<WebAPIClient, WebAPIClient>();
            services.AddTransient<ClienteResolverServices, ClienteResolverServices>();

            //Aggiungiamo la cache in memory
            services.AddMemoryCache();

            //TODO: CONFIGURARE DPAPI PER USARE UN CERTIFICATO SU AZURE (STORAGE O VAULT)
            // vedi: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers#azure-and-redis
            //services.AddDataProtection()
            //    .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"));

          
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAdB2C(options => Configuration.Bind("Authentication:AzureAdB2C", options))
            .AddCookie();

            services.AddMvc();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SharedViewsLocationExpander());
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Necessario per usare Nginx come reverse proxy (altrimenti non funziona l'authentication)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "logout",
                    template: "logout",
                    defaults: new { controller = "Account", action = "Logout" });
                routes.MapRoute(
                    name: "users",
                    template: "users/myprofile",
                    defaults: new { controller = "Users", action = "Index" }
                    );
                routes.MapRoute(
                    name: "clienti",
                    template: "{cliente:required}/{controller=Clienti}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
