using FuncNotificationHandler.DataAccess;
using FuncNotificationHandler.Model;
using Newtonsoft.Json.Linq;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler.Handlers
{
    internal class NuovoAccountHandler : BaseNotificationHandler
    {
        private static List<string> Placeholders = new List<string>()
        {
            "CODE",
            "EMAIL",
            "URLATTIVAZIONE"
        };

        const int ID_TIPO_NOTIFICA = 1; //Account to confirm
        const string SYSTEM_NOTIFICA_TITOLO = "Account da confermare";
        //TODO: Definire il testo del messaggio
        const string SYSTEM_NOTIFICA_TESTO = "E' necessario confermare il proprio account prima di poter utilizzare a pieno tutte le funzionalità.";
        public NuovoAccountHandler(NotificationMessage message) : base(message)
        {

        }

        private async Task<MailTemplate> LoadTemplateAsync()
        {
            MailTemplate result = null;
            switch (_message.SubType.ToUpper())
            {
                case "CLIENTE":
                    result = await SqlDataAccessor.LoadMailTemplateAsync(TipologieMailTemplates.ConfermaCliente);
                    break;
                case "UTENTE":
                    result = await SqlDataAccessor.LoadMailTemplateAsync(TipologieMailTemplates.ConfermaUtente);
                    break;
            }
            return result;
        }

        private Dictionary<string, string> GetPlaceholders()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var ph in Placeholders)
            {
                result.Add(ph, _message.Properties[ph].ToString());
            }
            return result;
        }


        public async override Task HandleNotificationAsync()
        {
            // 01 - Invio email    
            var template = await LoadTemplateAsync();
            MailSender ms = new MailSender(template);
            JObject from = _message.Properties["FROM"] as JObject;
            JObject to = _message.Properties["TO"] as JObject;
            await ms.SendMailAsync(GetPlaceholders(),
                                    new EmailAddress((string)from["email"], (string)from["name"]),
                                    new EmailAddress((string)to["email"], (string)to["name"]));
            // 02 - Invio notifica a sistema
            await SqlDataAccessor.AddSystemNotificationAsync(ID_TIPO_NOTIFICA, _message.UsersId.SingleOrDefault(), _message.IdCliente, SYSTEM_NOTIFICA_TITOLO, SYSTEM_NOTIFICA_TESTO);
        }
        
    }
}
