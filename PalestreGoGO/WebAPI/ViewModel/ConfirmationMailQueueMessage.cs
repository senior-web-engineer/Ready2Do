using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PalestreGoGo.WebAPI.Model
{
    [JsonObject(Title ="confirmMail")]
    public class ConfirmationMailQueueMessage
    {
        public ConfirmationMailQueueMessage()
        {

        }

        public ConfirmationMailQueueMessage(string userName, string code, string token)
        {
            this.UserName = userName;
            this.ConfirmationCode = code;
            this.CorrelationToken = token;
        }

        [JsonProperty("username",Required =Required.Always)]
        public string UserName { get; set; }

        [JsonProperty("code", Required = Required.Always)]
        public string ConfirmationCode { get; set; }

        [JsonProperty("token", Required = Required.Always)]
        public string CorrelationToken { get; set; }
    }
}
