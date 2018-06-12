using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public abstract class NuovoAppuntamentoBaseViewModel 
    {

        [JsonProperty("evento", Required = Required.Always)]
        public int IdEvento { get; set; }

        [JsonProperty("note", Required = Required.AllowNull)]
        public string Note { get; set; }
    }

    public class NuovoAppuntamentoViewModel: NuovoAppuntamentoBaseViewModel
    {
        [JsonProperty("user", Required = Required.Always)]
        public Guid IdUtente { get; set; }

    }

    public class NuovoAppuntamentoGuestViewModel: NuovoAppuntamentoBaseViewModel
    {
        [JsonProperty("nominativo", Required = Required.Always)]
        public string Nominativo { get; set; }
    }
}
