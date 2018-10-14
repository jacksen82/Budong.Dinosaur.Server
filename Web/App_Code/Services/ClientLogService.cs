using System;
using System.IO;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端日志操作类
/// </summary>
public class ClientLogService
{
    /// <summary>
    /// 记录客户端日志
    /// </summary>
    /// <param name="session3rd">string 三方标识</param>
    public static void Append(string session3rd)
    {
        int scene = Parse.ToInt(HttpContext.Current.Request.Params["scene"]);
        string url = HttpContext.Current.Request.Url.AbsolutePath;
        string query = String.Empty;
        foreach (string key in HttpContext.Current.Request.Form.AllKeys)
        {
            query += key + "=" + HttpContext.Current.Request.Form[key] + "&";
        }
        foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
        {
            query += key + "=" + HttpContext.Current.Request.QueryString[key] + "&";
        }
        if (Parse.ToString(HttpContext.Current.Application[session3rd]) != url + query)
        {
            HttpContext.Current.Application[session3rd] = url + query;
            File.AppendAllText(Budong.Common.Utils.Files.MapPath("~/logs/log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"), "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + session3rd + ", " + scene + ", " + url + ", " + query + "\r\n");
        }
    }
}