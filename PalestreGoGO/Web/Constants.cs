using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web
{
    public static class Constants
    {

        public const string ClaimStructureOwned = "extension_struttureOwned";
        public const string ClaimStructureManaged = "extension_struttureGestite";
        public const string ClaimEmailConfirmedOn = "extension_accountConfirmedOn";
        public const string ClaimTelephoneConfirmedOn = "extension_telephoneConfirmedOn";
        public const string ClaimGlobalAdmin = "extension_isGlobalAdmin";
        public const string ClaimRole = "role";
        public const string ClaimUserId = ClaimTypes.NameIdentifier;
        public const string ClaimPolicy = "tfp";
        public const string ClaimNome = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string ClaimCognome = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string ClaimDisplayName = "name";
        public const string ClaimEmail = "emails";

        public const string ROLE_GLOBAL_ADMIN = "global_admin";

        public const string CUSTOM_HEADER_TOKEN_AUTH = "X-PalestreGoGO-AUTHToken";

    }
}
