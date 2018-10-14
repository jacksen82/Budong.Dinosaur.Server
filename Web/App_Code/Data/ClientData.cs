using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端操作类
/// </summary>
public class ClientData
{
    /// <summary>
    /// 根据 ClientId 获取用户信息
    /// </summary>
    /// <param name="clientId">int 用户编号</param>
    /// <returns>Hash 用户信息</returns>
    public static Hash GetByClientId(int clientId)
    {
        string sql = "SELECT *, "+
            "   (SELECT COUNT(*) FROM tc_client_notify WHERE clientId=tc_client.clientId AND readed=0) AS unreadCount " +
            "FROM tc_client WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, clientId);
        }
    }
    /// <summary>
    /// 根据 OpenId 获取用户信息
    /// </summary>
    /// <param name="openId">string 微信标识</param>
    /// <returns>Hash 用户信息</returns>
    public static Hash GetByOpenId(string openId)
    {
        string sql = "SELECT *, " +
            "   (SELECT COUNT(*) FROM tc_client_notify WHERE clientId=tc_client.clientId AND readed=0) AS unreadCount " +
            "FROM tc_client WHERE openId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, openId);
        }
    }
    /// <summary>
    /// 根据三方会话标识获取用户信息
    /// </summary>
    /// <param name="session3rd">string 三方会话标识</param>
    /// <returns>Hash 用户信息</returns>
    public static Hash GetBySession3rd(string session3rd)
    {
        string sql = "SELECT *, " +
            "   (SELECT COUNT(*) FROM tc_client_notify WHERE clientId=tc_client.clientId AND readed=0) AS unreadCount " +
            "FROM tc_client WHERE session3rd=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, session3rd);
        }
    }
    /// <summary>
    /// 生成一个新的客户端编号
    /// </summary>
    /// <returns>int 新客户端编号</returns>
    public static int GetNewClientId()
    {
        string sql = "SELECT MAX(clientId) FROM tc_client";
        using (MySqlADO ado = new MySqlADO())
        {
            return Parse.ToInt(ado.GetValue(sql), 1000000000) + 1;
        }
    }
    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="openId">string 微信标识</param>
    /// <param name="sessionKey">string 微信会话标识</param>
    /// <param name="session3rd">string 三方会话标识</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, string openId, string sessionKey, string session3rd)
    {
        string sql = "INSERT INTO tc_client (clientId,openId,sessionKey,session3rd) VALUES(@0,@1,@2,@3) ON DUPLICATE KEY UPDATE openId=VALUES(openId),sessionKey=VALUES(sessionKey),session3rd=VALUES(session3rd),updateTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, openId, sessionKey, session3rd);
        }
    }
    /// <summary>
    /// 设置客户端资料
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="nick">string 昵称</param>
    /// <param name="gender">int 性别</param>
    /// <param name="avatarUrl">string 头像图片</param>
    /// <returns>int 受影响的行数</returns>
    public static int SetUserInfo(int clientId, string nick, int gender, string avatarUrl)
    {
        string sql = "UPDATE tc_client SET nick=@1,gender=@2,avatarUrl=@3,actived=1 WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, nick, gender, avatarUrl);
        }
    }
}