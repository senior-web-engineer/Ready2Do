using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class MessagesService : IEmailSender, ISMSSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //TODO: Implementare            
            throw new NotImplementedException();
        }

        public async Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
        }
    }
}
