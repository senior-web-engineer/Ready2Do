using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using PalestreGoGo.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class UserConfirmationService : IUserConfirmationService
    {
        private readonly IConfiguration _config;

        private CloudQueueClient _queueClient;
        private string _queueName;

        public UserConfirmationService(IConfiguration config)
        {
            _config = config;
            _queueName = _config.GetValue<string>("Azure:ConfirmationMailQueueName");
        }

        private void EnsureQueueClient()
        {
            if (_queueClient != null) return;
            CloudStorageAccount account = CloudStorageAccount.Parse(_config.GetValue<string>("Azure:ConfirmationMailQueueConnString"));
            this._queueClient = account.CreateCloudQueueClient();
        }

        public async Task EnqueueConfirmationMailRequest(ConfirmationMailQueueMessage mailReq)
        {
            EnsureQueueClient();            
            var queue = _queueClient.GetQueueReference(_queueName);
            await queue.CreateIfNotExistsAsync();
            var msg = JsonConvert.SerializeObject(mailReq);
            await queue.AddMessageAsync(new CloudQueueMessage(msg));
        }
    }
}
