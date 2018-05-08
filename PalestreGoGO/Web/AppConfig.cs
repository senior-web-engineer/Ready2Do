using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Configuration
{
    public class AppConfig
    {
        public AppConfig()
        {
            AuthTokenDuration = 30; //Per default 30 minuti
        }

        public string RegistrationUrl { get; set; }
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string EncryptKey { get; set; }
        public int AuthTokenDuration { get; set; }
        public WebAPIConfig WebAPI { get; set; }
        public STSConfig STS { get; set; }
        public GoogleAPIConfig GoogleAPI { get; set; }
        public AzureConfig Azure { get; set; }
        public MemoryCacheConfig CacheConfig { get; set; }
    }

    public class WebAPIConfig
    {
        public string BaseAddress { get; set; }
    }

    public class STSConfig
    {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResponseType { get; set; }
        public string[] Scopes { get; set; }
    }

    public class GoogleAPIConfig
    {
        public string GoogleMapsAPIKey { get; set; }
    }

    public class AzureConfig
    {
        public class StorageConfig
        {
            public string AccountName { get; set; }
            public string AccountKey { get; set; }
            public string ConnectionString { get; set; }
            public string BlobStorageBaseUrl { get; set; }
        }
        public string SendGridAPIKey { get; set; }
        public StorageConfig Storage { get; set; }
    }

    public class MemoryCacheConfig
    {
        public int ClienteSlidingExpiration { get; set; }
    }
}
