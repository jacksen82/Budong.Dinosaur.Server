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
        string sql = "SELECT * FROM tc_client WHERE clientId=@0";
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
        string sql = "SELECT * FROM tc_client WHERE openId=@0";
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
        string sql = "SELECT * FROM tc_client WHERE session3rd=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, session3rd);
        }
    }
    /// <summary>
    /// 根据客户端获取排行榜
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>Hash 排行榜</returns>
    public static Hash GetRank(int clientId)
    {
        string sql = "SELECT *,tcr.relateType,tcr.openGId " +
            "FROM tc_client tc LEFT JOIN tc_client_relate tcr ON tc.clientId=tcr.toClientId AND tcr.fromClientId=@0 " +
            "WHERE tc.actived=1 ORDER BY tc.score DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollection(sql, clientId);
        }
    }
    /// <summary>
    /// 根据客户端获取排位信息
    /// </summary>
    /// <param name="clientId">int 邀请客户端编号</param>
    /// <param name="score">int 成绩</param>
    /// <returns>Hash 排名信息</returns>
    public static Hash GetRank(int clientId, int score)
    {
        string sql = "SELECT ((SELECT COUNT(*) FROM tc_client WHERE score<@1)*100)/(SELECT COUNT(*) FROM tc_client) AS position ";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, clientId, score);
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
        string sql = "INSERT INTO tc_client (clientId,openId,sessionKey,session3rd) " +
            "VALUES(@0,@1,@2,@3) " +
            "ON DUPLICATE KEY UPDATE openId=VALUES(openId),sessionKey=VALUES(sessionKey),session3rd=VALUES(session3rd),updateTime=Now()";
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
    /// <summary>
    /// 重新开始
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Restart(int clientId)
    {
        string sql = "UPDATE tc_client SET duration=0,score=0,status=0 WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId);
        }
    }
    /// <summary>
    /// 复活继续
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Continue(int clientId)
    {
        string sql = "UPDATE tc_client SET cards=cards-1,status=0 WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId);
        }
    }
    /// <summary>
    /// 答题结果
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="result">int 答题结果</param>
    /// <returns>int 受影响的行数</returns>
    public static int Answer(int clientId, int result)
    {
        string sql = "UPDATE tc_client SET " +
            "   duration=(SELECT COUNT(*) FROM tc_client_dinosaur WHERE clientId=@0 AND result=1), " +
            "   score=IF(@1=1,score+1,score), " +
            "   status=IF(@1=1,0,200) " +
            "WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, result);
        }
    }
}