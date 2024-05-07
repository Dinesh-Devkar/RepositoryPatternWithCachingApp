using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Configurations;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheConfiguration _cacheConfiguration;

        private  MemoryCacheEntryOptions _cacheEntryOptions;

        public MemoryCacheService(IMemoryCache memoryCache,IOptions<CacheConfiguration> cacheConfiguration)
        {
            this._memoryCache = memoryCache;
            this._cacheConfiguration = cacheConfiguration.Value;

            //Setting MemoryCache Entry Options
            if (_cacheConfiguration!=null)
            {
                _cacheEntryOptions=new MemoryCacheEntryOptions{
                    AbsoluteExpiration=DateTime.Now.AddMinutes(_cacheConfiguration.AbsoluteExpirationInHours),
                    Priority=CacheItemPriority.High,
                    SlidingExpiration=TimeSpan.FromMinutes(_cacheConfiguration.SlidingExpirationInMinutes)
                };
            }
        }
        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public T Set<T>(string cacheKey, T value)
        {
            return _memoryCache.Set(cacheKey,value,_cacheEntryOptions);
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            _memoryCache.TryGetValue(cacheKey, out value);
            if (value==null)
            {
                return false;
            }
            return true;
        }
    }
}