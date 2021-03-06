using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class IndirizzoViewModel
    {
        [JsonProperty("indirizzo")]
        public string Indirizzo { get; set; }

        [JsonProperty("citta")]
        public string Citta { get; set; }

        [JsonProperty("cap")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("coordinate")]
        public CoordinateAPIModel Coordinate { get; set; }
    }
}
