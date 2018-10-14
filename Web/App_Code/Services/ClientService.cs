using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端业务操作类
/// </summary>
public class ClientService
{
    /// <summary>
    /// 获取 Token 方法
    /// </summary>
    /// <param name="session3rd">string 三方标识</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Token(string session3rd)
    {
        if (Genre.IsNull(session3rd))
        {
            return new Hash((int)CodeType.Session3rdRequired, "session3rd 为空");
        }

        Hash data = new Hash();
        Hash client = ClientData.GetBySession3rd(session3rd);

        if (client.ToInt("clientId") > 0)
        {
            return new Hash((int)CodeType.OK, "成功", client);
        }
        return new Hash((int)CodeType.ClientNotExists, "session3rd 无效");
    }
    /// <summary>
    /// 登录方法
    /// </summary>
    /// <param name="code">string 微信登录凭证</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Login(string code)
    {
        if (Genre.IsNull(code))
        {
            return new Hash((int)CodeType.CodeRequired, "code 为空");
        }
        Hash setting = SettingData.Detail();
        Hash wechatSession = API.Code2Session(setting.ToString("appKey"), setting.ToString("appSecret"), code);
        if (wechatSession.ToInt("errcode") != 0)
        {
            return new Hash((int)CodeType.CodeInvalid, wechatSession.ToString("errmsg"));
        }
        int clientId = ClientData.GetNewClientId();
        string openId = wechatSession.ToString("openid");
        string sessionKey = wechatSession.ToString("session_key");

        if (ClientData.Create(clientId, openId, sessionKey, Guid.NewGuid().ToString().Replace("-", "")) > 0)
        {
            return new Hash((int)CodeType.OK, "成功", ClientData.GetByOpenId(openId));
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
    }
    /// <summary>
    /// 设置客户端资料
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="nick">string 昵称</param>
    /// <param name="gender">int 性别</param>
    /// <param name="avatarUrl">string 头像</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash SetUserInfo(Hash token, string nick, int gender, string avatarUrl)
    {
        if (ClientData.SetUserInfo(token.ToInt("clientId"), nick, gender, avatarUrl) > 0)
        {
            return new Hash((int)CodeType.OK, "成功", ClientData.GetByClientId(token.ToInt("clientId")));
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
    }
}