using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Utils
{
    public static class ClaimUtils
    {
        public static bool IsGlobalAdmin(this ClaimsPrincipal principal)
        {
            //TODO: Verificare come modellare questa info
            if (principal == null) return false;
            if (principal.Claims.Any(c => c.Type.Equals(Constants.ClaimRole) && c.Value.Equals(Constants.RoleGlobalAdmin))) return true;
            return false;
        }

        /*Ritorna una lista di IdClienti per cui il principal è l'owner*/
        public static IEnumerable<int> StructureOwned(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var clienti = principal.Claims.Where(c => c.Type.Equals(Constants.ClaimStructureOwned)).Select(c=>int.Parse(c.Value));
            return clienti ;
        }

        /*Ritorna una lista di IdClienti per cui il principal ha i permessi amministrativi */
        public static IEnumerable<int> StructureManaged(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var clienti = principal.Claims.Where(c => c.Type.Equals(Constants.ClaimStructureManaged)).Select(c => int.Parse(c.Value));
            return clienti;
        }

        public static bool CanEditTipologiche(this ClaimsPrincipal principal, int idCliente)
        {
            return true;
            if (principal == null) return false;
            if (principal.IsGlobalAdmin()) return false;
            IEnumerable<int> clienti = principal.StructureOwned();
            if (clienti.Contains(idCliente)) return true;
            clienti = principal.StructureManaged();
            if (clienti.Contains(idCliente)) return true;
            return false;
        }

        public static bool CanReadTipologiche(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return false;
            if (principal.IsGlobalAdmin()) return false;
            IEnumerable<int> clienti = principal.StructureOwned();
            if (clienti.Contains(idCliente)) return true;
            clienti = principal.StructureManaged();
            if (clienti.Contains(idCliente)) return true;
            return false;
        }

    }
}
