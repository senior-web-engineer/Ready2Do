using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace Web
{
    public class Program
    {
        public static IConfiguration GetConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddCommandLine(args) //aggiunto per gestire il parametro --urls "http://xxxx" in sede di hosting per evitare di usare la porta 5000 di default
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("hosting.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args) //aggiunto per gestire il parametro --urls "http://xxxx" in sede di hosting per evitare di usare la porta 5000 di default
            .Build();
        }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(GetConfiguration(args))
            .UseSerilog((ctx, cfg) => cfg.ReadFrom.ConfigurationSection(ctx.Configuration.GetSection("Serilog")).Enrich.FromLogContext())
            .UseStartup<Startup>()
            .Build();
    }
}
