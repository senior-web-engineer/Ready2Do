using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
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

        [JsonProperty("accountEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled { get; set; }

        [JsonProperty("creationType", NullValueHandling = NullValueHandling.Ignore)]
        public string CreationType { get; private set; }

        [JsonProperty("givenName", NullValueHandling = NullValueHandling.Ignore)]
        public string Nome { get; set; }

        [JsonProperty("surname", NullValueHandling = NullValueHandling.Ignore)]
        public string Cognome { get; set; }

        [JsonProperty("displayName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("mobile", NullValueHandling = NullValueHandling.Ignore)]
        public string Mobile { get; private set; }

        [JsonProperty("otherMails", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Emails { get; set; }

        [JsonProperty("passwordProfile", NullValueHandling = NullValueHandling.Ignore)]
        public PasswordProfile PasswordProfile { get; set; }

        [JsonProperty("preferredLanguage", NullValueHandling = NullValueHandling.Ignore)]
        public string PreferredLanguage { get; set; }
        
        [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode { get; private set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; private set; }

        [JsonProperty("streetAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string streetAddress { get; private set; }

        [JsonProperty("signInNames", NullValueHandling = NullValueHandling.Ignore)]
        public List<SignInName> SignInNames { get; set; }

        [JsonProperty("userPrincipalName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserPrincipalName { get; set; }

        [JsonProperty("telephoneNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string TelephoneNumber { get; set; }

        #region Extensions Property
        /*
         ATTENZIONE! Il nome serializzato degli attributi custom sono specifici del Tenant in cui si scrive per cui cambiano ad ogni deploy
         //TODO: Sarà pertanto necessario customizzare la serializzazione di queste proprietà
         */

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_refereer", NullValueHandling = NullValueHandling.Ignore)]
        public string Refereer { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_emailConfirmationDate", NullValueHandling = NullValueHandling.Ignore)]
        public string EmailConfirmedOn { get; set; }


        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_struttureOwned", NullValueHandling = NullValueHandling.Ignore)]
        public string StruttureOwned { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_struttureGestite", NullValueHandling = NullValueHandling.Ignore)]
        public string StruttureGestite { get; set; }

        [JsonProperty("extension_827f6baba88543a9952b028ac0e17bf9_isGlobalAdmin", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsGlobalAdmin { get; set; }
        #endregion

        public AzureUser()
        {
        }

        public AzureUser(string email, string password) : this()
        {
            // Deve essere valorizzate sempre a LocalAccount
            this.CreationType = "LocalAccount";
            this.Enabled = true;
            this.SignInNames = new List<SignInName>();
            this.Emails = new List<string>();
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
