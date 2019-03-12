using Newtonsoft.Json;
using System;

namespace ready2do.model.common
{
    public class AppuntamentoDaConfermareDM : AppuntamentoBaseDM
    {
        /// <summary>
        /// Id dell'eventuale appuntamento se confermato
        /// </summary>
        public int? IdAppuntamento { get; set; }

        public DateTime? DataEsito { get; set; }
        public string MotivoRifiuto { get; set; }
        public DateTime? DataExpiration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ScheduleDM Schedule { get; set; }

        /// <summary>
        /// Indica se l'appunamento può essere confermato o meno.
        /// Al momento un appuntamento è confermabile se esiste un abbonamento compatibile
        /// </summary>
        public bool? CanBeConfirmed { get; set; }
    }
}
