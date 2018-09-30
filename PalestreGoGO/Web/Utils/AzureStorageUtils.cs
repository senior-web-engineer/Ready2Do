using System;
using System.Net;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Web.Configuration;
using System.Threading.Tasks;

namespace Web.Utils
{
    public class AzureStorageUtils
    {
        public static String GetSasForBlob(AzureConfig config, string blobUri, string verb, int validity=15)
        {
            var credentials = new StorageCredentials(config.Storage.AccountName, config.Storage.AccountKey);
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUri), credentials);
            var permission = verb == "DELETE" ? SharedAccessBlobPermissions.Delete: SharedAccessBlobPermissions.Write;

            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = permission,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(validity),
            });            
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas);
        }

        public async static Task EnsureContainerExists(AzureConfig config, string containerName)
        {
            var credentials = new StorageCredentials(config.Storage.AccountName, config.Storage.AccountKey);
            CloudBlobClient client = new CloudBlobClient(new Uri(config.Storage.BlobStorageBaseUrl), credentials);
            var contRef = client.GetContainerReference(containerName);
            //Ritorna True se il container non esisteva ed è stato appena creato
            if (await contRef.CreateIfNotExistsAsync()){
                var permissions = await contRef.GetPermissionsAsync();
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                await contRef.SetPermissionsAsync(permissions);
            }
        }

        public static async Task DeleteBlobAsync(AzureConfig config, string blobUri)
        {
            var credentials = new StorageCredentials(config.Storage.AccountName, config.Storage.AccountKey);
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUri), credentials);
            await blob.DeleteIfExistsAsync();
        }
    }
}
