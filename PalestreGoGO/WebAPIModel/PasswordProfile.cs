using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class PasswordProfile
    {
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("forceChangePasswordNextLogin")]
        public bool ForceChangePasswordNextLogin { get; set; }
    }
}
