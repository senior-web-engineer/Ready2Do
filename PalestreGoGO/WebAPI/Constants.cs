using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string ClaimStructureOwned = "structure_owned";
        public const string ClaimStructureManaged = "structure_managed";
        public const string ClaimStructureAffiliated = "structure_affiliated";

        public const string ClaimUserId = "uid";
        #endregion

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
