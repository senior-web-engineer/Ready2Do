using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI
{
    public static class Constants
    {        
        #region FORMATS
        public const string DATETIME_QUERYSTRING_FORMAT = "yyyyMMddTHHmmss";

        #endregion

        #region CLAIMS
        public const string ClaimRole = "role";

        public const string ClaimGlobalAdmin = "extension_isGlobalAdmin";
        public const string ClaimStructureOwned = "extension_struttureOwned";
        public const string ClaimStructureManaged = "extension_struttureGestite";
        public const string ClaimStructureAffiliated = "extension_refereer";
        public const string ClaimAccountConfirmedOn = "extension_accountConfirmedOn";

        public const string ClaimMailConfirmed = "";
        public const string ClaimUserId = ClaimTypes.NameIdentifier; //"uid";
        public const string ClaimUserEmail = "emails";
        #endregion

        public const int DEFAULT_VALIDATION_VALIDITY = 2880; //48 Ore se non configurato diversamente

        #region SCOPES
        public const string ScopeTipologicheEdit = "tipologiche.edit";
        public const string ScopeProvisioningClienti = "palestregogo.api.clienti.provisioning";

        #endregion

        #region ROLES
        public const string RoleGlobalAdmin = "global_admin";
        #endregion

        #region TIPI_IMMAGINI
        public const int TIPOIMMAGINE_LOGO = 1;
        public const int TIPOIMMAGINE_SFONDO = 2;
        public const int TIPOIMMAGINE_GALLERY = 3;

        #endregion
    }
}
