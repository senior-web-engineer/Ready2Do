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
            if (principal.Claims.Any(c => c.Type.Equals(Constants.ClaimGlobalAdmin) && c.Value.Equals(true.ToString(),StringComparison.InvariantCultureIgnoreCase))) return true;
            return false;
        }
        public static string UserId(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            string userId = principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimUserId))?.Value;
            //if (Guid.TryParse(userId, out result)) return result;
            return userId;
        }

        public static string Email(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimUserEmail))?.Value;
        }

        /*Ritorna una lista di IdClienti per cui il principal è l'owner*/
        public static IEnumerable<int> StructureOwned(this ClaimsPrincipal principal)
        {
            List<int> result = new List<int>();
            if (principal == null) return null;
            var claim = principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureOwned));
            if ((claim == null) || string.IsNullOrWhiteSpace(claim.Value)) return result;
            foreach(var value in claim.Value.Split(','))
            {
                result.Add(int.Parse(value));
            }
            return result;
        }

        /*Ritorna una lista di IdClienti per cui il principal ha i permessi amministrativi */
        public static IEnumerable<int> StructureManaged(this ClaimsPrincipal principal)
        {
            List<int> result = new List<int>();
            if (principal == null) return null;
            var claim = principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureManaged));
            if ((claim == null) || string.IsNullOrWhiteSpace(claim.Value)) return result;
            foreach (var value in claim.Value.Split(','))
            {
                result.Add(int.Parse(value));
            }
            return result;
        }

        public static bool CanEditTipologiche(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return false;
            if (principal.IsGlobalAdmin()) return true;
            IEnumerable<int> clienti = principal.StructureOwned();
            if (clienti.Contains(idCliente)) return true;
            clienti = principal.StructureManaged();
            if (clienti.Contains(idCliente)) return true;
            return false;
        }

        public static bool CanReadTipologiche(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return false;
            if (principal.IsGlobalAdmin()) return true;
            IEnumerable<int> clienti = principal.StructureOwned();
            if (clienti.Contains(idCliente)) return true;
            clienti = principal.StructureManaged();
            if (clienti.Contains(idCliente)) return true;
            return false;
        }

        public static bool CanManageStructure(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return false;
            if (principal.IsGlobalAdmin()) return true;
            IEnumerable<int> clienti = principal.StructureOwned();
            if (clienti.Contains(idCliente)) return true;
            clienti = principal.StructureManaged();
            if (clienti.Contains(idCliente)) return true;
            return false;

        }
    }
}
