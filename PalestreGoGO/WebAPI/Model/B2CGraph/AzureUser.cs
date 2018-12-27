using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel.B2CGraph
{
    public class AzureUser
    {
        [JsonProperty(PropertyName = "objectId", NullValueHandling= NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        [JsonProperty(PropertyName = "country", NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "createdDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDateTime { get; set; }

        [JsonProperty("accountEnabled")]
        public bool Enabled { get; set; }

        [JsonProperty("creationType")]
        public string CreationType { get; private set; }

        [JsonProperty("givenName")]
        public string Nome { get; set; }

        [JsonProperty("surname")]
        public string Cognome { get; set; }

        [JsonProperty("displayName", DefaultValueHandling = DefaultValueHandling.Include)]
        public string DisplayName { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; private set; }

        [JsonProperty("otherMails")]
        public List<string> Emails { get; set; }

        [JsonProperty("passwordProfile")]
        public PasswordProfile PasswordProfile { get; set; }

        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }
        
        [JsonProperty("postalCode")]
        public string PostalCode { get; private set; }

        [JsonProperty("state")]
        public string State { get; private set; }

        [JsonProperty("streetAddress")]
        public string streetAddress { get; private set; }

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

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_refereer")]
        public string Refereer { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_accountConfirmedOn")]
        public string AccountConfirmedOn { get; set; }


        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_struttureOwned")]
        public string StruttureOwned { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_struttureGestite")]
        public string StruttureGestite { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_isGlobalAdmin")]
        public bool IsGlobalAdmin { get; set; }
        #endregion

        public AzureUser()
        {
            // Deve essere valorizzate sempre a LocalAccount
            this.CreationType = "LocalAccount";
            this.Enabled = true;
            this.SignInNames = new List<SignInName>();
            this.Emails = new List<string>();
        }

        public AzureUser(string email, string password) : this()
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
