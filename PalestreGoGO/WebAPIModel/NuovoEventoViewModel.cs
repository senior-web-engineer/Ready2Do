using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{

    public class NuovoEventoViewModel
    {
        [JsonProperty("user", Required = Required.Always)]
        public Guid IdUtente { get; set; }

        [JsonProperty("evento", Required = Required.Always)]
        public int IdEvento { get; set; }

        [JsonProperty("note", Required = Required.AllowNull)]
        public string Note { get; set; }
    }

    public class NuovoEventoGuestViewModel: NuovoEventoViewModel
    {
        [JsonProperty("nominativo", Required = Required.Always)]
        public string Nominativo { get; set; }
    }
}
