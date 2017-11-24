using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FuncSendUserMailConfirmation
{
    public static class SendConfirmationMail
    {
        [FunctionName("SendConfirmationMail")]
        public static void Run([QueueTrigger("confirmation-mails")]string myQueueItem,
                                int dequeueCount,
                                [SendGrid] out Mail message,
                                TraceWriter log)
        {
            log.Info($"Executing function SendConfirmationMail. Item=[{myQueueItem}], dequeueCount: {dequeueCount}");
            //TODO: DA IMPLEMENTARE
            if (dequeueCount < 2) throw new ApplicationException("First attempt always fails!");

            message = new Mail(new Email("gianluca.tofi@outlook.com"),
                               "Prova email SendGrid",
                               new Email("gianlucatofi@gmail.com", "Gianluca Tofi"),
                               new Content("text/html", "content"));
        }
    }
}
