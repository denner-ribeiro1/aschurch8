using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace ASChurchManager.Web.Lib
{
    public static class CacheHelper
    {
        public static void Clear(this MemoryCache cache) 
        {
            MethodInfo clearMethod = cache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo prop = cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop != null)
            {
                object innerCache = prop.GetValue(cache);
                if (innerCache != null)
                {
                    clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(innerCache, null);
                    }
                }
            }
        }
    }
}
