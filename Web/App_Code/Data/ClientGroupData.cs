using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端群关系操作类
/// </summary>
public class ClientGroupData
{
    /// <summary>
    /// 创建群关系条目
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="openGId">string 群标识</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, string openGId)
    {
        string sql = "INSERT INTO tc_client_group (clientId, openGId) VALUES(@0, @1) ON DUPLICATE KEY UPDATE clientId=VALUES(clientId), openGId=VALUES(openGId), updateTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, openGId);
        }
    }
    /// <summary>
    /// 更新群关系条目
    /// </summary>
    /// <param name="openGId">string 群标识</param>
    /// <returns>int 受影响的行数</returns>
    public static int Renovate(string openGId)
    {
        string sql = "INSERT INTO tc_client_relate(fromClientId, toClientId, relateType, openGId) " +
            "SELECT a.clientId, b.clientId,@1,@0 FROM tc_client_group a, tc_client_group b " +
            "WHERE a.openGId=@0 AND b.openGId=@0 AND a.clientId!=b.clientId AND NOT EXISTS ( " +
            "   SELECT* FROM tc_client_relate WHERE fromClientId=a.fromClientId and toClientId = b.toClientId " +
            ") ";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, openGId, (int)RelateType.FromGroup);
        }
    }
}