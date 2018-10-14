using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 故事操作类
/// </summary>
public class StoryData
{
    /// <summary>
    /// 根据 storyId 获取故事信息
    /// </summary>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>Hash 故事信息</returns>
    public static Hash GetByStoryId(int storyId)
    {
        string sql = "SELECT * FROM ts_story WHERE storyId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, storyId);
        }
    }
    /// <summary>
    /// 获取故事列表
    /// </summary>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 故事集合</returns>
    public static Hash List(int pageId, int pageSize)
    {
        string sql = "SELECT ts.*,tsr.createTime AS relayTime,tc.nick,tc.gender,tc.avatarUrl " +
            "FROM ts_story ts LEFT JOIN ts_story_relay tsr ON ts.storyId = tsr.storyId AND ts.lastStoryRelayId = tsr.storyRelayId " +
            "   LEFT JOIN tc_client tc ON tsr.clientId = tc.clientId "+
            "WHERE ts.deleted=0 " +
            "ORDER BY ts.updateTime DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollectionByPageId(sql, pageId, pageSize);
        }
    }
    /// <summary>
    /// 获取故事列表
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 故事集合</returns>
    public static Hash List(int clientId, int pageId, int pageSize)
    {
        string sql = "SELECT * FROM ts_story WHERE clientId=@0 AND deleted=0 ORDER BY updateTime DESC";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollectionByPageId(sql, pageId, pageSize, clientId);
        }
    }
    /// <summary>
    /// 生成一个新的故事编号
    /// </summary>
    /// <returns>int 新故事编号</returns>
    public static int NewStoryId()
    {
        string sql = "SELECT MAX(storyId) FROM ts_story";
        using (MySqlADO ado = new MySqlADO())
        {
            return Parse.ToInt(ado.GetValue(sql), 2000000000) + 1;
        }
    }
    /// <summary>
    /// 创建一个新故事
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Create(int clientId, int storyId)
    {
        string sql = "INSERT INTO ts_story (clientId,storyId) VALUES(@0,@1) ON DUPLICATE KEY UPDATE storyId=VALUES(storyId),clientId=VALUES(clientId),updateTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, storyId);
        }
    }
    /// <summary>
    /// 重设故事参数
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Reset(int clientId, int storyId)
    {
        string sql = "UPDATE ts_story SET lastStoryRelayId=(SELECT MAX(storyRelayId) FROM ts_story_relay WHERE storyId=@0 AND deleted=0) WHERE storyId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, storyId);
        }
    }
    /// <summary>
    /// 删除故事
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Delete(int clientId, int storyId)
    {
        string sql = "UPDATE ts_story SET deleted=1 WHERE storyId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, storyId);
        }
    }
}