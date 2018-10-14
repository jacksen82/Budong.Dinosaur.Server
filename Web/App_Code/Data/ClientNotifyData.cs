using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端消息操作类
/// </summary>
public class ClientNotifyData
{
    /// <summary>
    /// 获取客户端关注故事列表
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 关注集合</returns>
    public static Hash List(int clientId, int pageId, int pageSize)
    {
        string sql = "SELECT tsr.*,tcn.notifyType,tcn.readed,tc.nick,tc.gender,tc.avatarUrl " +
            "FROM tc_client_notify tcn LEFT JOIN ts_story_relay tsr ON tcn.storyId=tsr.storyId AND tcn.storyRelayId=tsr.storyRelayId " +
            "   LEFT JOIN tc_client tc ON tcn.notifyClientId=tc.clientId " +
            "WHERE tcn.clientId=@0 AND tsr.deleted=0 " +
            "ORDER BY tcn.updateTime DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollectionByPageId(sql, pageId, pageSize, clientId);
        }
    }
    /// <summary>
    /// 全部已读
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int ReadAll(int clientId)
    {
        string sql = "UPDATE tc_client_notify SET readed=1 WHERE clientId=@0 AND readed=0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId);
        }
    }
    /// <summary>
    /// 故事被关注消息
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Bookmark(int clientId, int storyId, int storyRelayId)
    {
        //  通知续作作者，通知故事作者
        string sql = "INSERT INTO tc_client_notify (clientId,storyId,storyRelayId,notifyClientId,notifyType,readed) "+
            "   SELECT clientId,@1,@2,@0,@3,0 FROM ts_story_relay WHERE clientId<>@0 AND storyId=@1 AND storyRelayId=@2 AND parentStoryRelayId>0;" +
            "INSERT INTO tc_client_notify (clientId,storyId,storyRelayId,notifyClientId,notifyType,readed) " +
            "   SELECT clientId,@1,@2,@0,@3,0 FROM ts_story_relay WHERE clientId<>@0 AND storyId=@1 AND parentStoryRelayId=0;";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId, (int)NotifyType.StoryRelayBookmark);
        }
    }
    /// <summary>
    /// 故事被续写消息
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续作编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Relay(int clientId, int storyId, int storyRelayId)
    {
        //  通知续作作者，通知故事作者，通知关注客户端
        string sql = "INSERT INTO tc_client_notify (clientId,storyId,storyRelayId,notifyClientId,notifyType,readed) " +
            "   SELECT clientId,@1,@2,@0,@3,0 FROM ts_story_relay WHERE clientId<>@0 AND storyId=@1 AND storyRelayId=@2 AND parentStoryRelayId>0;" +
            "INSERT INTO tc_client_notify (clientId,storyId,storyRelayId,notifyClientId,notifyType,readed) " +
            "   SELECT clientId,@1,@2,@0,@3,0 FROM ts_story_relay WHERE clientId<>@0 AND storyId=@1 AND parentStoryRelayId=0;" +
            "INSERT INTO tc_client_notify (clientId,storyId,storyRelayId,notifyClientId,notifyType,readed) " +
            "   SELECT clientId,@1,@2,@0,@4,0 FROM tc_client_bookmark WHERE clientId<>@0 AND storyId=@1 AND storyRelayId=@2;";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId, storyRelayId, (int)NotifyType.StoryRelayRelay, (int)NotifyType.BookmarkRelay);
        }
    }
}