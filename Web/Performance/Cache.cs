using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;

namespace Cfe.Web.Performance
{
    public static class Cache
    {
        private static ObjectCache cache = MemoryCache.Default; 

        public static void Add(string key, object obj)
        {
            // Creo las reglas del cache
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.SlidingExpiration = TimeSpan.FromMinutes(10);

            cache.Set(key, obj, policy);
        }

        public static void Remove(string key)
        {
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        public static bool Exists(string key)
        {
            return cache.Contains(key);
        }

        public static object Get(string key)
        {
            return cache.Get(key);
        }
    }
}