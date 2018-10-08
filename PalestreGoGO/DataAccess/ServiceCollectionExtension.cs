using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDataAccessRepositories(this IServiceCollection services ,Action<DataAccessRepositoriesOptions> configure)
        {
            var config = new DataAccessRepositoriesOptions();
            if (configure != null) configure(config);

            services.AddDbContext<PalestreGoGoDbContext>(ctxOpt =>
            {
                ctxOpt.UseSqlServer(config.ConnectionString);
            });

            services.AddTransient<ITipologieAbbonamentiRepository, TipologieAbbonamentiRepository>();
            services.AddTransient<ITipologieLezioniRepository, TipologieLezioniRepository>();
            services.AddTransient<IClientiRepository, ClientiRepository>();
            services.AddTransient<IAbbonamentiRepository, AbbonamentiRepository>();
            services.AddTransient<IAppuntamentiRepository, AppuntamentiRepository>();
            services.AddTransient<IClientiRepository, ClientiRepository>();
            services.AddTransient<ILocationsRepository, LocationsRepository>();
            services.AddTransient<ISchedulesRepository, SchedulesRepository>();
            services.AddTransient<ITipologieClientiRepository, TipologieClientiRepository>();
            services.AddTransient<IMailTemplatesRepository, MailTemplatesRepository>();
            services.AddTransient<IUtentiRepository, UtentiRepository>();

            return services;
        }
    }
}
