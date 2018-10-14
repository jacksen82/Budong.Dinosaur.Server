using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 故事续作操作类
/// </summary>
public class StoryRelayData
{
    /// <summary>
    /// 获取续作详情
    /// </summary>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续作编号</param>
    /// <returns>Hash 续作详情</returns>
    public static Hash GetByStoryRelayId(int storyId, int storyRelayId)
    {
        string sql = "SELECT tsr.*,tc.nick,tc.gender,tc.avatarUrl " +
            "FROM ts_story_relay tsr LEFT JOIN tc_client tc ON tsr.clientId=tc.clientId " +
            "WHERE tsr.storyId=@0 AND tsr.storyRelayId=@1";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, storyId, storyRelayId);
        }
    }
    /// <summary>
    /// 获取指定故事全部续作
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>Hash 续作集合</returns>
    public static Hash All(int clientId, int storyId)
    {
        string sql = "SELECT tsr.*,tc.nick,tc.gender,tc.avatarUrl, " +
            "   (SELECT IF(COUNT(*)>0,1,0) FROM tc_client_bookmark WHERE clientId=@0 AND storyId=@1 AND storyRelayId=tsr.storyRelayId) AS bookmarked " +
            "FROM ts_story_relay tsr LEFT JOIN tc_client tc ON tsr.clientId=tc.clientId " +
            "WHERE tsr.storyId=@1 AND tsr.deleted=0 " +
            "ORDER BY tsr.parentStoryRelayId ASC, (tsr.bookmarkCount + tsr.relayCount) DESC ";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollection(sql, clientId, storyId);
        }
    }
    /// <summary>
    /// 获取指定客户端全部续写
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 续作集合</returns>
    public static Hash List(int clientId, int pageId, int pageSize)
    {
        string sql = "SELECT tsr.*,ts.content AS origin,tc.nick,tc.gender,tc.avatarUrl " +
            "FROM ts_story_relay tsr LEFT JOIN tc_client tc ON tsr.clientId=tc.clientId " +
            "   LEFT JOIN ts_story ts ON tsr.storyId=ts.storyId "+
            "WHERE tsr.clientId=@0 AND tsr.parentStoryRelayId>0 AND tsr.deleted=0 " +
            "ORDER BY tsr.updateTime DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollectionByPageId(sql, pageId, pageSize, clientId);
        }
    }
    /// <summary>
    /// 生成一个新的故事续作编号
    /// </summary>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>int 新故事续作编号</returns>
    public static int NewStoryRelayId(int storyId)
    {
        string sql = "SELECT MAX(storyRelayId) FROM ts_story_relay WHERE storyId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return Parse.ToInt(ado.GetValue(sql, storyId), 10000000) + 1;
        }
    }
    /// <summary>
    /// 创建一个新故事续作
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <param name="parentStoryRelayId">int 故事上级续作编号 </param>
    /// <param name="content">string 续作内容</param>
    /// <param name="depth">int 续写深度</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, int storyId, int storyRelayId, int parentStoryRelayId, string content, int depth)
    {
        string sql = "INSERT INTO ts_story_relay (clientId,storyId,storyRelayId,parentStoryRelayId,content,depth) VALUES(@0,@1,@2,@3,@4,@5) ON DUPLICATE KEY UPDATE storyId=VALUES(storyId),storyRelayId=VALUES(storyRelayId),clientId=VALUES(clientId),content=VALUES(content),updateTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId, parentStoryRelayId, content, depth);
        }
    }
    /// <summary>
    /// 更新统计
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续写故事编号</param>
    /// <param name="parentStoryRelayId">int 上级续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Recount(int clientId, int storyId, int storyRelayId, int parentStoryRelayId)
    {
        string sql = "UPDATE tc_client SET relayCount=(SELECT COUNT(*) FROM ts_story_relay WHERE clientId=tc_client.clientId) WHERE clientId=@0;";
        sql += "UPDATE ts_story SET relayCount=(SELECT COUNT(*) FROM ts_story_relay WHERE storyId=ts_story.storyId) WHERE storyId=@1;";
        if (parentStoryRelayId == 0)
        {
            sql += "UPDATE ts_story SET content=(SELECT content FROM ts_story_relay WHERE storyId=@1 AND storyRelayId=@2),firstStoryRelayId=@2,lastStoryRelayId=@2 WHERE storyId=@1;";
            sql += "UPDATE ts_story_relay SET relayCount=(SELECT COUNT(*) FROM ( SELECT COUNT(*) FROM ts_story_relay WHERE storyId=@1 AND parentStoryRelayId=@2) tsr ) WHERE storyId=@1 AND storyRelayId=@2;";
        }
        else
        {
            sql += "UPDATE ts_story SET lastStoryRelayId=@2 WHERE storyId=@1;";
        }
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId);
        }
    }
    /// <summary>
    /// 删除故事续作
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Delete(int clientId, int storyId, int storyRelayId)
    {
        string sql = "UPDATE ts_story_relay SET deleted=1 WHERE storyId=@0 AND storyRelayId=@1";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, storyId, storyRelayId);
        }
    }
}  