using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PalestreGoGo.WebAPI.Model
{
    [JsonObject(Title ="confirmMail")]
    public class ConfirmationMailMessage
    {
        public ConfirmationMailMessage()
        {

        }

        public ConfirmationMailMessage(string email, string code, /*string token,*/ bool isCliente)
        {
            this.Email = email;
            this.ConfirmationCode = code;
            //this.CorrelationToken = token;
            this.IsCliente = isCliente;
        }

        [JsonProperty("email",Required =Required.Always)]
        public string Email { get; set; }

        [JsonProperty("code", Required = Required.Always)]
        public string ConfirmationCode { get; set; }

        [JsonProperty("confirmationurl", Required = Required.Always)]
        public string ConfirmationUrl { get; set; }

       /* [JsonProperty("token", Required = Required.Always)]
        public string CorrelationToken { get; set; }*/

        [JsonProperty("isCliente", Required = Required.Always)]
        public bool IsCliente{ get; set; }

    }
}
