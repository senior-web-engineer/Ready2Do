using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace PalestreGoGo.WebAPI
{
    public class Program
    {
        public static IConfiguration GetConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
           .AddJsonFile("hosting.json", optional: true, reloadOnChange: true) // per gestire la configurazione degli URL Kestrel da file di configurazione
           .AddEnvironmentVariables()
           .AddCommandLine(args) //aggiunto per gestire il parametro --urls "http://xxxx" in sede di hosting per evitare di usare la porta 5000 di default
           .Build();
        }

        public static void Main(string[] args)
        {
            //IConfiguration configuration = GetConfiguration(args);

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("DEBUG", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                Serilog.Debugging.SelfLog.Enable(Console.Error);
                    }
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Metodo per la creazione dell IWebHostBuilder.
        /// Deve essere definito in questo modo perché referenziato dalla classe WebApplicationFactory utilizzata nei tests di integrazione
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(GetConfiguration(args))
                .UseSerilog((ctx, cfg) =>cfg.ReadFrom.ConfigurationSection(ctx.Configuration.GetSection("Serilog")).Enrich.FromLogContext(), false)
                .UseStartup<Startup>()
                .UseApplicationInsights();
        }

        public static IWebHost BuildWebHost(string[] args) => CreateWebHostBuilder(args).Build();
    }
}
