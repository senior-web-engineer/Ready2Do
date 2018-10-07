using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.B2CGraph
{
    public enum SignInNameType
    {
        userName,
        emailAddress
    }

    public class SignInName
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public SignInName(SignInNameType type)
        {
            switch (type)
            {
                case SignInNameType.emailAddress:
                    this.Type = "emailAddress";
                    break;
                case SignInNameType.userName:
                    this.Type = "userName";
                    break;
            }
        }
    }
}
