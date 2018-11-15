using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler
{
    internal class AccountToConfirmHandler
    {
        private SendGridMessage buildMailMessage(MailTemplate template, ConfirmationMailMessage mailReq)
        {
            var placeHolders = GetPlaceholdersValues(mailReq);
            var from = new EmailAddress(_config.GetValue<string>("Email:FromAddress"), _config.GetValue<string>("Email:FromName"));
            var to = new EmailAddress(mailReq.Email);
            template.Subject = ReplacePlaceHolders(template.Subject, placeHolders);
            template.HtmlBody = ReplacePlaceHolders(template.HtmlBody, placeHolders);
            template.TextBody = ReplacePlaceHolders(template.TextBody, placeHolders);
            var result = MailHelper.CreateSingleEmail(from, to, template.Subject, template.TextBody, template.HtmlBody);
            return result;
        }
    }
}
