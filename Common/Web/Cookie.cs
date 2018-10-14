using System;
using System.Web;

namespace Budong.Common.Web
{
    /// <summary>
    /// Cookie 封装类
    /// </summary>
    public class Cookie
    {
        /// <summary>
        /// 设置 Cookie 
        /// </summary>
        /// <param name="name">string 名称</param>
        /// <param name="value">string 值</param>
        /// <param name="days">int 过期天数</param>
        public static void Set(string name, string value, int days)
        {
            if (value == null)
            {
                HttpCookie cookie = new HttpCookie(name, value);
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
            else
            {
                if (HttpContext.Current.Request.Cookies[name] != null) HttpContext.Current.Response.Cookies.Remove(name);
                HttpCookie cookie = new HttpCookie(name, value);
                cookie.Path = "/";
                cookie.Expires = DateTime.Now.AddDays(days);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }
        /// <summary>
        /// 获取 Cookie 
        /// </summary>
        /// <param name="name">string 名称</param>
        /// <returns>string 值</returns>
        public static string Get(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                return HttpContext.Current.Request.Cookies[name].Value;
            }
            return string.Empty;
        }
    }
    /// <summary>
    /// Session 封装类
    /// </summary>
    public class Session
    {
        /// <summary>
        /// 设置 Session
        /// </summary>
        /// <param name="name">string 名称</param>
        /// <param name="value">object 值</param>
        public static void Set(string name, object value)
        {
            HttpContext.Current.Session[name] = value;
        }
        /// <summary>
        /// 获取 Session
        /// </summary>
        /// <param name="name">string 名称</param>
        /// <returns>object 值</returns>
        public static object Get(string name)
        {
            return HttpContext.Current.Session[name];
        }
    }
}
