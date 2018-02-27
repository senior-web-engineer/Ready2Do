using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Utils
{
    public static  class SecurityUtils
    {
        private const short KEY_SIZE = 256;

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
        public static UserType GetUserTypeForCliente(this ClaimsPrincipal principal, int idCliente)
        {
            if (principal == null) return UserType.Anonymous;
            if (!principal.Identity.IsAuthenticated) return UserType.Anonymous; // Se non autenticato ==> 
            UserType result = UserType.Anonymous;
            //Valutiamo il cliam USERS_MANAGED per determinare se l'utente corrente è un Admin per il cliente
            foreach (var c in principal.Claims.Where(c => c.Type.Equals(Constants.ClaimType_Users_Managed)))
            {
                foreach(var cliente in c.Value.Split(','))
                {
                    if (int.Parse(cliente).Equals(idCliente))
                    {
                        result = UserType.Admin;
                    }
                }
            }
            //TODO: AGGIUNGERE GESTIONE GLOBAL ADMIN
            return result;
        }
    }
}
