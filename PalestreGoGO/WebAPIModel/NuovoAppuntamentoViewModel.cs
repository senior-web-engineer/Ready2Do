using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public abstract class NuovoAppuntamentoBaseApiModel 
    {

        [JsonProperty("evento", Required = Required.Always)]
        public int IdEvento { get; set; }

        [JsonProperty("note", Required = Required.AllowNull)]
        public string Note { get; set; }
    }

    public class NuovoAppuntamentoApiModel: NuovoAppuntamentoBaseApiModel
    {
        [JsonProperty("user", Required = Required.Always)]
        public string IdUtente { get; set; }

    }

    public class NuovoAppuntamentoGuestApiModel: NuovoAppuntamentoBaseApiModel
    {
        [JsonProperty("nominativo", Required = Required.Always)]
        public string Nominativo { get; set; }
    }
}
