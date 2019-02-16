using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler
{
    public class NotificationMessage
    {
        [JsonProperty("type")]
        public string TipoNotifica { get; set; }

        [JsonProperty("subType")]
        public string SubType { get; set; }

        [JsonProperty("idCliente")]
        public int IdCliente { get; set; }

        [JsonProperty("users")]
        public string[] UsersId { get; set; }
        [JsonProperty("props")]
        public Dictionary<string, object> Properties { get; set; }
    }
}
