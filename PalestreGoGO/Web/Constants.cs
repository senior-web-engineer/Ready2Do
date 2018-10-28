using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web
{
    public static class Constants
    {

        public const string ClaimStructureOwned = "structure_owned";
        public const string ClaimStructureManaged = "structure_managed";
        public const string ClaimAccountConfirmedOn = "structure_managed";
        public const string ClaimRole = "role";
        public const string ClaimUserId = ClaimTypes.NameIdentifier;
        public const string ClaimPolicy = "tfp";
        public const string ROLE_GLOBAL_ADMIN = "global_admin";

        public const string CUSTOM_HEADER_TOKEN_AUTH = "X-PalestreGoGO-AUTHToken";

    }
}
