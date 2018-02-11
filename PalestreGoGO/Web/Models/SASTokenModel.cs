using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class SASTokenModel
    {
        [JsonProperty(PropertyName ="st")]
        public string SecurityToken { get; set; }

        [JsonProperty(PropertyName = "ct")]
        public DateTime CreationTime { get; set; }

        [JsonProperty(PropertyName = "cn")]
        public string ContainerName { get; set; }

    }
}
