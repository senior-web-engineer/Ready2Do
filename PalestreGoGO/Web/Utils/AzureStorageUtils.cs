using System;
using System.Net;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Web.Configuration;

namespace Web.Utils
{
    public class AzureStorageUtils
    {
        public static String GetSasForBlob(AzureConfig config, string blobUri, string verb, int validity=15)
        {
            var credentials = new StorageCredentials(config.Storage.StorageAccountName, config.Storage.StorageAccountKey);
            CloudBlockBlob blob = new CloudBlockBlob(new Uri(blobUri), credentials);
            var permission = verb == "DELETE" ? SharedAccessBlobPermissions.Delete: SharedAccessBlobPermissions.Write;

            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = permission,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(validity),
            });            
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sas);
        }

    }
}
