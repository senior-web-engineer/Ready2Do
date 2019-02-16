using Microsoft.Extensions.DependencyInjection;
using System;

namespace PalestreGoGo.DataAccess
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDataAccessRepositories(this IServiceCollection services ,Action<DataAccessRepositoriesOptions> configure)
        {
            var config = new DataAccessRepositoriesOptions();
            configure?.Invoke(config);
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
            services.AddTransient<INotificheRepository, NotificheRepository>();
            services.AddTransient<IClientiUtentiRepository, ClientiUtentiRepository>();
            services.AddTransient<IImmaginiClientiRepository, ImmaginiClientiRepository>();
            services.AddTransient<IRichiesteRegistrazioneRepository, RichiesteRegistrazioneRepository>();
            return services;
        }
    }
}
