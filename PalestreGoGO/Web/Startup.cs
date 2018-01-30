using System;
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
            //services.Configure<STSConfig>(Configuration);
            services.Configure<WebAPIConfig>(Configuration.GetSection("WebAPIConfig"));
            services.Configure<ApplicationConfigurations>(Configuration.GetSection("ApplicationConfigurations"));
            services.Configure<GoogleAPIConfig>(Configuration.GetSection("GoogleAPIConfig"));

            // Add application services.
            services.AddTransient<AccountServices, AccountServices>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = Configuration["STSConfig:Authority"];
                options.RequireHttpsMetadata = bool.Parse(Configuration["STSConfig:RequireHttpsMetadata"]);
                options.ClientId = Configuration["STSConfig:ClientId"];
                options.ClientSecret = Configuration["STSConfig:ClientSecret"];
                options.ResponseType = Configuration["STSConfig:ResponseType"];
                var scopes = Configuration.GetSection("STSConfig:Scopes").GetChildren().Select(x => x.Value);
                foreach (var s in scopes)
                {
                    options.Scope.Add(s);
                }
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
            });
            services.AddMvc();
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
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
