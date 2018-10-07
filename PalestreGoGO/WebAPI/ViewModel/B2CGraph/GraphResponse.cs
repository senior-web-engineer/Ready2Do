using Newtonsoft.Json;
using PalestreGoGo.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.B2CGraph
{
    public class GraphResponse
    {
        [JsonProperty("odata.metadata")]
        public string ODATAMetadata { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(BlobJsonConverter))]
        public string Value { get; set; }
    }
}
