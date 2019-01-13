using Common.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Utils
{
    public static class SecurityUtils
    {
        private const short KEY_SIZE = 256;

        /// <summary>
        /// Genera una stringa rappresentante un "token" per l'autenticazione delle chiamate Ajax
        /// </summary>
        /// <param name="secuirtyToken"></param>
        /// <param name="storageContainer"></param>
        /// <returns></returns>
        public static string GenerateSASAuthenticationToken(int idCliente, string storageContainer, string encryptKey)
        {
            var token = new SASTokenModel()
            {
                IdCliente = idCliente,
                ContainerName = storageContainer,
                CreationTime = DateTime.Now
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(encryptKey));
            return result;
        }

        public static string GenerateAuthenticationToken(string clientRouteUrl, int idCliente, string encryptKey)
        {
            var token = new AuthTokenModel()
            {
                ClientRoute = clientRouteUrl,
                CreationTime = DateTime.Now,
                IdCliente = idCliente
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(encryptKey));
            return result;
        }

        public static string EncryptStringWithAes(string plainText, byte[] Key)
        {
            if ((Key == null) || (Key.Length != (KEY_SIZE / 8))) throw new ArgumentException(nameof(Key));

            byte[] encrypted;
            byte[] IV;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                IV = aesAlg.IV;
                aesAlg.Mode = CipherMode.CBC;
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            var combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);
            // Return the encrypted bytes from the memory stream. 
            return Convert.ToBase64String(combinedIvCt);
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key)
        {
            if ((Key == null) || (Key.Length != (KEY_SIZE / 8))) throw new ArgumentException(nameof(Key));
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                byte[] IV = new byte[aesAlg.BlockSize / 8];
                byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];
                Array.Copy(cipherTextCombined, IV, IV.Length);
                Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption. 
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }

    }

    public static class ClaimsPrinciapalExtensions
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
