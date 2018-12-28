using Newtonsoft.Json;
using System;

namespace ready2do.model.common
{
    public class FasciaOrariaAperturaDM
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public TimeSpan? Inizio { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public TimeSpan? Fine { get; set; }
    }
}
