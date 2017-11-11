using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS.Services
{
    /*
     * Servizio invio email
     * TODO: Aggiungere gestione templates
     */
    public interface IEmailSender
    {
        //TODO: Verificare se deve ritornare un esito
        Task SendEmailAsync(string email, string subject, string message);
    }
}
