using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid;
using SendGrid.Helpers.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Azure.Func.SendAccountConfirmationEmail
{
    public static class FuncSendAccountConfirmationEmail
    {
        [FunctionName("R2DSendAccountConfirmationEmail")]
        public static async Task Run([QueueTrigger("confirmation-mails", Connection = "QUEUE_CONNECTION")]ConfirmationEmailQueueMessage item, TraceWriter log)
        {
            try
            {
                log.Info($"C# Queue trigger function processed: {item}");
                var key = ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
                var sgClient = new SendGridClient(key);
                var msg = new SendGridMessage();
                msg.SetTemplateId(ConfigurationManager.AppSettings["SENDGRID_ACCOUNTCONFIRM_TEMPLATEID"]);
                msg.SetFrom(new EmailAddress("support@ready2do.com", "Ready2Do"));
                msg.AddTo(item.Email);
                msg.SetTemplateData(new
                {
                    nome = item.Nome,
                    cognome = item.Cognome,
                    confirmationurl = item.ConfirmationUrl
                });
                log.Verbose($"Sending confirmation email to: {item.Email}");
                var sendResult = await sgClient.SendEmailAsync(msg);
                log.Info($"Confirmation email sent to : {item.Email}");
            }
            catch (Exception exc)
            {
                log.Error("Errore durante l'invio dell'email", exc);
                throw;
            }
        }
    }
}