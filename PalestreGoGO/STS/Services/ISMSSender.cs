using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS.Services
{
    public interface ISMSSender
    {
        //TODO: Verificare se deve ritornare un eisto
        Task SendSmsAsync(string number, string message);
    }
}
