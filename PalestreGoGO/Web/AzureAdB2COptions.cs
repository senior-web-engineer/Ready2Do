
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";
        

        public AzureAdB2COptions()
        {
            //AzureAdB2CInstance = "https://login.microsoftonline.com/tfp";
            AzureAdB2CInstance = "https://ready2do.b2clogin.com";
        }

        public string ClientId { get; set; }
        public string AzureAdB2CInstance { get; set; }
        public string Tenant { get; set; }
        /* POLICY STRUTTURE */
        public string StrutturaSignInPolicyId { get; set; }
        public string StrutturaResetPasswordPolicyId { get; set; }
        /* FINE POLICY STRUTTURE */

        /* POLICY PER GLI UTENTI */
        public string UserSignUpSignInPolicyId { get; set; }
        //public string UserSignInPolicyId { get; set; }
        //public string UserSignUpPolicyId { get; set; }
        public string UserResetPasswordPolicyId { get; set; }
        //public string UserEditProfilePolicyId { get; set; }
        /* FINE POLOCY UTENTI */
        public string RedirectUri { get; set; }

        public string DefaultPolicy => StrutturaSignInPolicyId;
        public string Authority(string policyName) => $"{AzureAdB2CInstance}/{Tenant}/{policyName}/v2.0";

        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }
    }
}
