using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class ClienteFollowedApiModel
    {
        [JsonProperty("id")]
        public int IdCliente { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("ragioneSociale")]
        public string RagioneSociale { get; set; }

        [JsonProperty("dataFollowing")]
        public DateTime DataFollowing { get; set; }

        [JsonProperty("abbonamento")]
        public bool AbbonamentoValido { get; set; }
    }
}
