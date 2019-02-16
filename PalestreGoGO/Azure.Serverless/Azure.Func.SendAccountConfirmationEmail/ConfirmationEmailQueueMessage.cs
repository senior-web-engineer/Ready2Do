using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Func.SendAccountConfirmationEmail
{
    public class ConfirmationEmailQueueMessage
    {
        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }

        [JsonProperty("code", Required = Required.Always)]
        public string ConfirmationCode { get; set; }

        [JsonProperty("confirmationurl", Required = Required.Always)]
        public string ConfirmationUrl { get; set; }

        [JsonProperty("isCliente", Required = Required.Always)]
        public bool IsCliente { get; set; }

        [JsonProperty("nome", Required = Required.DisallowNull)]
        public string Nome { get; set; }

        [JsonProperty("cognome", Required = Required.DisallowNull)]
        public string Cognome { get; set; }
    }
}
