using System;
using System.Web;
using System.Web.Caching;

namespace Budong.Common.Web
{
    /// <summary>
    /// 服务器缓存
    /// </summary>
    public class Cache
    {
        /// <summary>
        /// 获取指定键名的 Cache 内容
        /// </summary>
        /// <param name="key">string Cache 键名</param>
        /// <returns>object Cache 内容</returns>
        public static object Get(string key)
        {
            if (HttpContext.Current.Cache.Get(key) == null ||
                HttpContext.Current.Cache[key] == null ||
                HttpContext.Current.Cache[key].ToString() == "")
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Cache[key];
            }
        }
        /// <summary>
        /// 设置指定键名的 Cache 内容
        /// </summary>
        /// <param name="key">string Cache 键名</param>
        /// <param name="obj">object Cache 内容</param>
        /// <param name="expira">DateTime Cache 过期时间</param>
        public static void Set(string key, object obj, DateTime expira)
        {
            if (obj == null)
            {
                HttpContext.Current.Cache.Remove(key);
            }
            else
            {
                if (HttpContext.Current.Cache.Get(key) == null)
                {
                    HttpContext.Current.Cache.Add(key, obj, null, expira, TimeSpan.Zero, CacheItemPriority.Default, null);
                }
                else
                {
                    HttpContext.Current.Cache[key] = obj;
                }
            }
        }
    }
}
