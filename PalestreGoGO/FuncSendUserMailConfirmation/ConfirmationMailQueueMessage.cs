using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncSendUserMailConfirmation
{
    public class ConfirmationMailQueueMessage
    {
        [JsonProperty("username", Required = Required.Always)]
        public string UserName { get; set; }

        [JsonProperty("code", Required = Required.Always)]
        public string ConfirmationCode { get; set; }

        [JsonProperty("token", Required = Required.Always)]
        public string CorrelationToken { get; set; }
    }
}
