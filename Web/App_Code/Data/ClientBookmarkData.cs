using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端关注操作类
/// </summary>
public class ClientBookmarkData
{
    /// <summary>
    /// 获取客户端对指定故事的关注信息
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>Hash 关注信息</returns>
    public static Hash GetByStoryRelayId(int clientId, int storyId, int storyRelayId)
    {
        string sql = "SELECT * FROM tc_client_bookmark WHERE clientId=@0 AND storyId=@1 AND storyRelayId=@2";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, clientId, storyId, storyRelayId);
        }
    }
    /// <summary>
    /// 获取客户端关注故事列表
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 关注集合</returns>
    public static Hash List(int clientId, int pageId, int pageSize)
    {
        string sql = "SELECT tsr.*,ts.content AS origin " +
            "FROM tc_client_bookmark tcb LEFT JOIN ts_story_relay tsr ON tcb.storyId=tsr.storyId AND tcb.storyRelayId=tsr.storyRelayId " +
            "   LEFT JOIN ts_story ts ON tsr.storyId=ts.storyId " +
            "WHERE tcb.clientId=@0 AND tsr.deleted=0 " +
            "ORDER BY tcb.updateTime DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollectionByPageId(sql, pageId, pageSize, clientId);
        }
    }
    /// <summary>
    /// 加为关注
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, int storyId, int storyRelayId)
    {
        string sql = "INSERT INTO tc_client_bookmark (clientId,storyId,storyRelayId) VALUES(@0,@1,@2) ON DUPLICATE KEY UPDATE clientId=VALUES(clientId),storyId=VALUES(storyId),storyRelayId=VALUES(storyRelayId),updateTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId);
        }
    }
    /// <summary>
    /// 更新统计
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续写故事编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Recount(int clientId, int storyId, int storyRelayId)
    {
        string sql = "UPDATE tc_client SET bookmarkCount=(SELECT COUNT(*) FROM tc_client_bookmark WHERE clientId=tc_client.clientId) WHERE clientId=@0;";
        sql += "UPDATE ts_story SET bookmarkCount=(SELECT COUNT(*) FROM tc_client_bookmark WHERE storyId=ts_story.storyId) WHERE storyId=@1;";
        sql += "UPDATE ts_story_relay SET bookmarkCount=(SELECT COUNT(*) FROM tc_client_bookmark WHERE storyId=ts_story_relay.storyId AND storyRelayId=ts_story_relay.storyRelayId) WHERE storyId=@1 AND storyRelayId=@2;";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId);
        }
    }
    /// <summary>
    /// 取消关注
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Delete(int clientId, int storyId, int storyRelayId)
    {
        string sql = "DELETE FROM tc_client_bookmark WHERE clientId=@0 AND storyId=@1 AND storyRelayId=@2";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId);
        }
    }
}