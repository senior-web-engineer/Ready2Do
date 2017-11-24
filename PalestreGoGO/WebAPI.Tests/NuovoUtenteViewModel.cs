using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class NuovoUtenteViewModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("cognome")]
        public string Cognome { get; set; }

        [JsonProperty("phone")]
        public string Telefono { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

    }
}
