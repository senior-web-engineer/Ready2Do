using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AuthTokenModel
    {
        [JsonProperty(PropertyName = "cr")]
        public string ClientRoute { get; set; }

        [JsonProperty(PropertyName = "cid")]
        public int IdCliente { get; set; }

        [JsonProperty(PropertyName = "ct")]
        public DateTime CreationTime { get; set; }
    }

    public class SASTokenModel
    {
        [JsonProperty(PropertyName ="id")]
        public int IdCliente { get; set; }

        [JsonProperty(PropertyName = "ct")]
        public DateTime CreationTime { get; set; }

        [JsonProperty(PropertyName = "cn")]
        public string ContainerName { get; set; }

    }
}
