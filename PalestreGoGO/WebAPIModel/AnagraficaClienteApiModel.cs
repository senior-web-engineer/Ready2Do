using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class AnagraficaClienteApiModel
    {
        [JsonProperty("id")]
        public int IdCliente { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("ragioneSociale")]
        public string RagioneSociale { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string NumTelefono { get; set; }

        [JsonProperty("desc")]
        public string Descrizione { get; set; }

        [JsonProperty("indirizzo")]
        public IndirizzoViewModel Indirizzo { get; set; }

        [JsonProperty("route")]
        public string UrlRoute { get; set; }

    }

}
