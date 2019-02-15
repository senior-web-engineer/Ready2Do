using Common.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Models;

namespace Web.Utils
{
    public static class ClaimsPrincipalExtensions
    {
        public static UserType GetUserTypeForCliente(this ClaimsPrincipal principal, string idCliente)
        {
            return principal.GetUserTypeForCliente(int.Parse(idCliente));
        }

        public static UserType GetUserTypeForCliente(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return UserType.Anonymous;
            if (!principal.Identity.IsAuthenticated) return UserType.Anonymous; // Se non autenticato ==> 
            UserType result = UserType.NormalUser;
            //Valutiamo i claim STRUCTURE_OWNED e STRUCTURE_MANAGED per determinare se l'utente corrente è un Admin per il cliente
            foreach (var c in principal.Claims.Where(c => c.Type.Equals(Constants.ClaimStructureManaged)))
            {
                foreach (var cliente in c.Value.Split(','))
                {
                    if (int.Parse(cliente).Equals(idCliente))
                    {
                        result = result | UserType.Admin;
                    }
                }
            }
            foreach (var c in principal.Claims.Where(c => c.Type.Equals(Constants.ClaimStructureOwned)))
            {
                foreach (var cliente in c.Value.Split(','))
                {
                    if (int.Parse(cliente).Equals(idCliente))
                    {
                        result = result | UserType.Owner;
                    }
                }
            }
            if (principal.Claims.Any(c => c.Type.Equals(Constants.ClaimGlobalAdmin) && c.Value.Equals(true.ToString())))
            {
                result = result | UserType.GlobalAdmin;
            }
            return result;
        }

        public static IEnumerable<int> StruttureGestite(this ClaimsPrincipal principal)
        {
            List<int> result = new List<int>();
            var claim = principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureManaged));
            if (claim == null) return result;
            foreach (var c in claim.Value.Split(','))
            {
                result.Add(int.Parse(c));
            }
            return result;
        }

        public static IEnumerable<int> StruttureOwned(this ClaimsPrincipal principal)
        {
            List<int> result = new List<int>();
            var claim = principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureOwned));
            if (claim == null) return result;
            foreach (var c in claim.Value.Split(','))
            {
                result.Add(int.Parse(c));
            }
            return result;
        }

        public static bool IsGlobalAdmin(this ClaimsPrincipal principal)
        {
            if (principal.Claims.Any(c => c.Type.Equals(Constants.ClaimGlobalAdmin) && c.Value.Equals(true.ToString())))
            {
                return true;
            }
            return false;
        }

        public static bool IsAtLeastAdmin(this UserType userType)
        {
            if ((userType & UserType.Admin) == UserType.Admin) return true;
            if ((userType & UserType.Owner) == UserType.Owner) return true;
            if ((userType & UserType.GlobalAdmin) == UserType.GlobalAdmin) return true;
            return false;
        }

        public static string UserId(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimUserId))?.Value;
        }

        public static string DisplayName(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimDisplayName))?.Value;
        }

        public static string Email(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimEmail))?.Value;
        }

        public static string Nome(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimNome))?.Value;
        }

        public static string Cognome(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            return principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimCognome))?.Value;
        }

        public static DateTime? EmailConfirmedOn(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            string value = principal.Claims.SingleOrDefault(c => c.Type.Equals(Constants.ClaimEmailConfirmedOn))?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    return long.Parse(value).FromUnixTimeSeconds();
                }
                catch
                {
                    Log.Error($"Errore durante la lettura del Claim {Constants.ClaimEmailConfirmedOn}, valore non valido [{value}]");
                }
            }
            return null;
        }
        public static DateTime? TelephoneConfirmedOn(this ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            string value = principal.Claims.SingleOrDefault(c => c.Type.Equals(Constants.ClaimTelephoneConfirmedOn))?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    return long.Parse(value).FromUnixTimeSeconds();
                }
                catch
                {
                    Log.Error($"Errore durante la lettura del Claim {Constants.ClaimTelephoneConfirmedOn}, valore non valido [{value}]");
                }
            }
            return null;
        }
        public static bool CanViewSidebar(this ClaimsPrincipal principal, int idCliente)
        {
            return principal.Identity.IsAuthenticated && principal.GetUserTypeForCliente(idCliente).IsAtLeastAdmin();
        }

    }
}
