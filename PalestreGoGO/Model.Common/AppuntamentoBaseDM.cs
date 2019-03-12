using Newtonsoft.Json;
using System;

namespace ready2do.model.common
{
    public abstract class AppuntamentoBaseDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UtenteClienteDM User { get; set; }

    }
}
