using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Authentication
{
    public class UserIdTokensMonitorService
    {
        //private Dictionary<string, List<Claim>> _usersToRefresh;
        private const string KEY_SUFFIX = "REFRESH_ID_TOKEN";
        private readonly IDistributedCache _distributedCache;
        private ReaderWriterLockSlim _rwLock;

        public UserIdTokensMonitorService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _rwLock = new ReaderWriterLockSlim();
        }
        public UserIdTokensMonitorService()
        {
            _rwLock = new ReaderWriterLockSlim();
        }

        public List<Claim> ClaimsToRefresh(string userId)
        {
            List<Claim> result = null;
            _rwLock.EnterReadLock();
            try
            {
                byte[] buffer = _distributedCache.Get(BuildKey(userId));
                if (buffer != null)
                {
                    result = DeserializeClaims(buffer);
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
            return result;
        }

        public void AddClaimToUserToRefresh(string userId, Claim claim)
        {
            _rwLock.EnterWriteLock();
            List<Claim> claims;
            byte[] buffer;
            try
            {
                buffer = _distributedCache.Get(BuildKey(userId));
                if (buffer == null) {
                    claims = new List<Claim>();
                }
                else
                {
                    claims = DeserializeClaims(buffer);
                }
                var oldClaim = claims.FirstOrDefault(c => c.Type.Equals(claim.Type));
                if (oldClaim != null)
                {
                    claims.Remove(oldClaim);
                }
                claims.Add(claim);
                _distributedCache.Set(BuildKey(userId), SerializeClaims(claims), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        public void RemoveUserToRefresh(string userId)
        {
            _rwLock.EnterWriteLock();
            try
            {
                _distributedCache.Remove(BuildKey(userId));
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private string BuildKey(string userId)
        {
            return $"{userId}_{KEY_SUFFIX}";
        }

        private byte[] SerializeClaims(List<Claim> claims)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, claims);
            return ms.ToArray();
        }

        private List<Claim> DeserializeClaims(byte[] obj)
        {
            List<Claim> result = default(List<Claim>);
            BinaryFormatter formatter = new BinaryFormatter();
            using (var ms = new MemoryStream(obj))
            {
                result = formatter.Deserialize(ms) as List<Claim>;
            }
            return result;
        }
    }
}
