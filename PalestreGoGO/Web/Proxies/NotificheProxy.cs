using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;

namespace Web.Proxies
{
    public class NotificheProxy : BaseAPIProxy
    {
        public NotificheProxy(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
                            IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions) :
            base(options, httpContextAccessor, distributedCache, authOptions)
        {

        }
    }
}
