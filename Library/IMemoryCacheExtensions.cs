using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public static class IMemoryCacheExtensions
    {
        public static T GetOrThrow<T>(this IMemoryCache cache, object key)
        {
            var retVal = cache.Get<T>(key);

            if (retVal == null)
            {
                throw new KeyNotFoundException($"Key {key} not found");
            }

            return retVal;
        }
    }
}
