using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using PalestreGoGo.DataAccess.Interfaces;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Model;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class UserConfirmationService : IUserConfirmationService
    {
        private static readonly Regex REGEX_MAIL_PLACEHOLDER = new Regex(@"(?i)\{\{(\w*)\}\}");

        private readonly IConfiguration _config;

        private CloudQueueClient _queueClient;
        private string _queueName;
        private IMailTemplatesRepository _mailRepository;
        private ILogger<UserConfirmationService> _logger;

        public UserConfirmationService(IConfiguration config, IMailTemplatesRepository mailRepository, ILogger<UserConfirmationService> logger)
        {
            _config = config;
            _queueName = _config.GetValue<string>("Azure:ConfirmationMailQueueName");
            this._mailRepository = mailRepository;
            this._logger = logger;
        }

        private void EnsureQueueClient()
        {
            if (_queueClient != null) return;
            CloudStorageAccount account = CloudStorageAccount.Parse(_config.GetValue<string>("Azure:ConfirmationMailQueueConnString"));
            this._queueClient = account.CreateCloudQueueClient();
        }

        public async Task EnqueueConfirmationMailRequestAsync(ConfirmationMailMessage mailReq)
        {
            EnsureQueueClient();
            var queue = _queueClient.GetQueueReference(_queueName);
            await queue.CreateIfNotExistsAsync();
            var msg = JsonConvert.SerializeObject(mailReq);
            await queue.AddMessageAsync(new CloudQueueMessage(msg));
        }

        public async Task SendConfirmationMailRequestAsync(ConfirmationMailMessage mailReq)
        {
            if (mailReq == null) throw new ArgumentNullException(nameof(mailReq));
            var template = mailReq.IsCliente ? await this._mailRepository.GetTemplateAsync(MailType.ConfermaCliente) : await this._mailRepository.GetTemplateAsync(MailType.ConfermaUtente);
            //var from = new EmailAddress(_config.GetValue<string>)

            var apiKey = _config.GetValue<string>("Email:SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var msg = buildMailMessage(template, mailReq);
            var response = await client.SendEmailAsync(msg);
            if(response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                string message = string.Format("Errore durante l'invio della mail di conferma. Response.StatusCode: {0}, Response. Body: {1}", response.StatusCode, await response.Body.ReadAsStringAsync());
                _logger.LogError(message);
                throw new ApplicationException(message);                
            }
        }

        private Dictionary<string,string> GetPlaceholdersValues(ConfirmationMailMessage mailReq)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Code".ToUpper(), WebUtility.UrlEncode(mailReq.ConfirmationCode));
            result.Add("Email".ToUpper(), mailReq.Email);
            result.Add("UrlAttivazione".ToUpper(), _config.GetValue<string>("UserConfirmation:ConfirmationBaseAddress"));
            //Eventualmente integrare leggendo dal DB
            return result;
        }

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

        private string ReplacePlaceHolders(string template, Dictionary<string, string> values)
        {
            var matches = REGEX_MAIL_PLACEHOLDER.Matches(template);
            Match m;
            string placeHolderName;
            if ((matches != null) && (matches.Count > 0))
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    m = matches[i];
                    placeHolderName = m.Groups[1].Value.ToUpper();
                    if (values.ContainsKey(placeHolderName))
                    {                        
                        template = template.Replace($"{{{{{placeHolderName}}}}}", values[placeHolderName], StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            return template;
        }
    }
}
