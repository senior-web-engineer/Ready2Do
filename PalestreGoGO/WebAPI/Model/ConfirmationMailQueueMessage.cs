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

        public ConfirmationMailMessage(string email, string code, string baseConfirmationUrl, bool isCliente, string cognome = null, string nome = null)
        {
            this.Email = email;
            this.Cognome = cognome;
            this.Nome = nome;
            this.ConfirmationCode = code;
            this.ConfirmationUrl= $"confirmatinUrl?email={System.Net.WebUtility.UrlEncode(email)}&code={System.Net.WebUtility.UrlEncode(code)}";
            this.IsCliente = isCliente;
            this.Timestamp = DateTime.Now;
        }

        [JsonProperty("email",Required =Required.Always)]
        public string Email { get; set; }

        [JsonProperty("code", Required = Required.Always)]
        public string ConfirmationCode { get; set; }

        [JsonProperty("confirmationurl", Required = Required.Always)]
        public string ConfirmationUrl { get; set; }

        [JsonProperty("isCliente", Required = Required.Always)]
        public bool IsCliente{ get; set; }

        [JsonProperty("nome", Required = Required.DisallowNull)]
        public string Nome { get; set; }

        [JsonProperty("cognome", Required = Required.DisallowNull)]
        public string Cognome { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
