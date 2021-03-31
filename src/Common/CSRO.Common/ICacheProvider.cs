namespace CSRO.Common
{
    public interface ICacheProvider
    {
        const int CacheSeconds = 300; //5 min

        T GetFromCache<T>(string key) where T : class;
        void SetCache<T>(string key, T value) where T : class;
        void SetCache<T>(string key, T value, int seconds = CacheSeconds) where T : class;
        void SetCache<T>(string key, string id, T value) where T : class;
        void SetCache<T>(string key, string id, T value, int seconds = CacheSeconds) where T : class;
        void ClearCache(string key);
        void ClearCache(string key, string id);
    }
}
