using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class WaitListRegistration
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdSchedule { get; set; }
        public int IdAbbonamento { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public DateTime? DataConversione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public byte? CausaleCancellazione { get; set; }
        public UtenteClienteDM User { get; set; }

        /// <summary>
        /// Lo schedule è valorizzato solo in alcuni casi (ad esempio quando si richiedono tutte le registrazioni in WaitList per un Utente)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ScheduleDM Schedule { get; set; }
    }
}
