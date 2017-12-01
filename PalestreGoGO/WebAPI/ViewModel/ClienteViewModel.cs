using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    /// <summary>
    /// Dettagli di un Cliente
    /// </summary>
    public class ClienteViewModel
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

        [JsonProperty("tipoCliente")]
        public TipologieClientiViewModel Tipologia { get; set; }

        [JsonProperty("indirizzo")]
        public IndirizzoViewModel Indrizzo { get; set; }

        [JsonProperty("orarioApertura")]
        public string OrarioApertura { get; set; }

        [JsonProperty("urlImgHome")]
        public string UrlImmagineHome { get; set; } 

        //public string 
    }
}
