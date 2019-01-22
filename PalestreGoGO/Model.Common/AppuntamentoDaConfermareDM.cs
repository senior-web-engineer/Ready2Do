using Newtonsoft.Json;
using System;

namespace ready2do.model.common
{
    public class AppuntamentoDaConfermareDM : AppuntamentoBaseDM
    {
        public DateTime? DataEsito { get; set; }
        public int? IdAppuntamentoConfermato { get; set; }
        public string MotivoRifiuto { get; set; }
        public DateTime? DataExpiration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ScheduleDM Schedule { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UtenteClienteDM User { get; set; }
    }
}
