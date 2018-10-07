using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.B2CGraph
{
    public class LocalAccountUser
    {
        [JsonProperty(PropertyName = "objectId", NullValueHandling= NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

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

        [JsonProperty("otherMails")]
        public List<string> Emails { get; set; }

        [JsonProperty("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; }

        [JsonProperty("signInNames")]
        public List<SignInName> SignInNames { get; set; }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("telephoneNumber")]
        public string TelephoneNumber { get; set; }

        #region Extensions Property
        /*
         ATTENZIONE! Il nome serializzato degli attributi custom sono specifici del Tenant in cui si scrive per cui cambiano ad ogni deploy
         //TODO: Sarà pertanto necessario customizzare la serializzazione di queste proprietà
         */

        [JsonProperty("extension_9ffd02bf4e6347e1b85574eef9015d78_Refereer")]
        public string Refereer { get; set; }

        [JsonProperty("extension_9ffd02bf4e6347e1b85574eef9015d78_EmailConfirmed")]
        public bool MailConfirmed { get; set; }

        #endregion

        public LocalAccountUser()
        {
            // Deve essere valorizzate sempre a LocalAccount
            this.CreationType = "LocalAccount";
            this.Enabled = true;
            this.SignInNames = new List<SignInName>();
            this.MailConfirmed = false;
            this.Emails = new List<string>();
        }

        public LocalAccountUser(string email, string password) : this()
        {
            this.SignInNames.Add(new SignInName(SignInNameType.emailAddress) { Value = email });
            this.Emails.Add(email);
            this.PasswordProfile = new PasswordProfile()
            {
                Password = password,
                ForceChangePasswordNextLogin = false
            };
        }
    }
}
