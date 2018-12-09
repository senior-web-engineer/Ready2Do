using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace PalestreGoGo.WebAPI
{
    public class Program
    {
        public static IConfiguration GetConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
           .AddJsonFile("hosting.json", optional: true, reloadOnChange: true) // per gestire la configurazione degli URL Kestrel da file di configurazione
           .AddEnvironmentVariables()
           .AddCommandLine(args) //aggiunto per gestire il parametro --urls "http://xxxx" in sede di hosting per evitare di usare la porta 5000 di default
           .Build();
        }

        public static void Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build()) //aggiunto per gestire il parametro --urls "http://xxxx" in sede di hosting per evitare di usare la porta 5000 di default
                .UseConfiguration(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("hosting.json", optional: true).Build()) // per gestire la configurazione degli URL Kestrel da file di configurazione
                .UseStartup<Startup>()
                .Build();
    }
}
