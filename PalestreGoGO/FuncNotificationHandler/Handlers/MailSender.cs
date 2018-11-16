using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler.Handlers
{
    public class MailSender
    {
        private MailTemplate _template;
        private SendGridClient _client;
        public MailSender(MailTemplate template)
        {
            _template = template;
            string sendgridKey = ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            if (string.IsNullOrWhiteSpace(sendgridKey))
            {
                throw new ApplicationException("Impossibile recuperare la API KEY per Sendgrid (KEY: SENDGRID_API_KEY)");
            }
            _client = new SendGridClient(sendgridKey);
        }

        public async Task SendMailAsync(Dictionary<string, string> placeholders, EmailAddress from, EmailAddress to)
        {
            SendGridMessage message = new SendGridMessage();
            message.SetFrom(from);
            message.AddTo(to);
            message.SetSubject(this.ResolvePlaceholders(_template.Subject, placeholders));
            if (!string.IsNullOrWhiteSpace(_template.HtmlBody))
            {
                message.AddContent(MimeType.Html, this.ResolvePlaceholders(_template.HtmlBody, placeholders));
            }
            if (!string.IsNullOrWhiteSpace(_template.TextBody))
            {
                message.AddContent(MimeType.Text, this.ResolvePlaceholders(_template.TextBody, placeholders));
            }


            var response = await _client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                string errMessage = string.Format("Errore durante l'invio della mail. Response.StatusCode: {0}, Response. Body: {1}", response.StatusCode, await response.Body.ReadAsStringAsync());
                throw new ApplicationException(errMessage);
            }
        }

        private string ResolvePlaceholders(string text, Dictionary<string, string> placeholders)
        {
            if (placeholders == null) return text;
            string result = text;
            foreach (var kv in placeholders)
            {
                result = result.Replace($"__{kv.Key}__", kv.Value);
            }
            return result;
        }
    }
}
