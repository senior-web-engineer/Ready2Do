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
        //[Required]
        //[EmailAddress]
        //[MaxLength(256)]
        [JsonProperty("email")]
        public string Email { get; set; }

        //[Required]
        //[MaxLength(100)]
        //[JsonProperty("nome")]
        public string Nome { get; set; }

        //[Required]
        //[MaxLength(100)]
        //[JsonProperty("cognome")]
        public string Cognome { get; set; }

        //[Phone]
        [JsonProperty("phone")]
        public string Telefono { get; set; }

        //[Required]
        [JsonProperty("password")]
        public string Password { get; set; }

    }
}
