using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.B2CGraph
{
    public class LocalAccountUser
    {
        [JsonProperty("accountEnabled")]
        public bool Enabled { get; set; }

        [JsonProperty("creationType")]
        public string CreationType { get; private set; }


        [JsonProperty("givenName")]
        public string Nome { get; set; }

        [JsonProperty("surname")]
        public string Cognome { get; set; }

        [JsonProperty("displayName", DefaultValueHandling = DefaultValueHandling.Include)]
        public string DisplayName
        {
            get
            {
                return $"{Nome} {Cognome}";
            }
        }

        [JsonProperty("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; }

        [JsonProperty("signInNames")]
        public List<SignInName> SignInNames { get; set; }

        [JsonProperty("telephoneNumber")]
        public string TelephoneNumber { get; set; }

        public LocalAccountUser()
        {
            // Deve essere valorizzate sempre a LocalAccount
            this.CreationType = "LocalAccount";
            this.Enabled = true;
            this.SignInNames = new List<SignInName>();
        }

        public LocalAccountUser(string email, string password): this()
        {
            this.SignInNames.Add(new SignInName(SignInNameType.emailAddress) { Value = email });
            this.PasswordProfile = new PasswordProfile()
            {
                Password = password,
                ForceChangePasswordNextLogin = false
            };
        }
    }
}
