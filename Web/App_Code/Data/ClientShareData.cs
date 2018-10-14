using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端分享操作类
/// </summary>
public class ClientShareData
{
    /// <summary>
    /// 分享记录
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="shareFrom">string 分享来源</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, string shareFrom)
    {
        string sql = "INSERT INTO tc_client_share (clientId, shareFrom) VALUES(@0, @1)";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, shareFrom);
        }
    }
}