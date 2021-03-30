﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Common
{
    public interface ICacheProvider
    {
        const int CacheSeconds = 300; //5 min

        T GetFromCache<T>(string key) where T : class;
        void SetCache<T>(string key, T value) where T : class;
        void SetCache<T>(string key, T value, int seconds = CacheSeconds) where T : class;
        void ClearCache(string key);
    }
    public class CacheProvider : ICacheProvider
    {        
        private readonly IMemoryCache _cache;

        public CacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = _cache.Get(key);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value) where T : class
        {
            SetCache(key, value, ICacheProvider.CacheSeconds);
        }

        public void SetCache<T>(string key, T value, int seconds = ICacheProvider.CacheSeconds) where T : class
        {
            _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(seconds));
        }

        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }
    }
}
