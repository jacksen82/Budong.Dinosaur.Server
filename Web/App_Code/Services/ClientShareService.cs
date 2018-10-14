using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端分享业务操作类
/// </summary>
public class ClientShareService
{
    /// <summary>
    /// 分享方法
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="shareFrom">string 分享来源</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Share(Hash token, string shareFrom)
    {
        if (ClientShareData.Create(token.ToInt("clientId"), shareFrom) > 0)
        {
            return new Hash((int)CodeType.OK, "成功");
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
    }
}