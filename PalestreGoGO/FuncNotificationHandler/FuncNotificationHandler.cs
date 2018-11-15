using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace FuncNotificationHandler
{
    public static class FuncNotificationHandler
    {
        [FunctionName("FuncNotificationHandler")]
        public static void Run([QueueTrigger("r2d-notifiche", Connection = "QUEUE_CONNECTION")]string notifica, TraceWriter log)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<NotificationMessage>(notifica);
                switch (message.TipoNotifica)
                {
                    case "AccountCreato":
                        break;
                }
            }
            catch (Exception ex) when (ex is JsonSerializationException|| ex is JsonReaderException)
            {
                log.Error($"Errore nella deserializzazione del messaggio di notifica: {notifica}",ex);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["QUEUE_CONNECTION"]);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("r2d-notifiche-poison");
                // Create the queue if it doesn't already exist.
                queue.CreateIfNotExists();
                CloudQueueMessage message = new CloudQueueMessage(notifica);
                queue.AddMessage(message);
            }
            log.Info($"C# Queue trigger function processed: {notifica}");
        }
    }
}
