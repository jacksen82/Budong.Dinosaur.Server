using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端故事续写业务操作类
/// </summary>
public class ClientStoryRelayService
{
    /// <summary>
    /// 获取故事列表
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash List(Hash token, int pageId, int pageSize)
    {
        return new Hash((int)CodeType.OK, "成功", StoryRelayData.List(token.ToInt("clientId"), pageId, pageSize));
    }
    /// <summary>
    /// 删除故事续写
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续写编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Delete(Hash token, int storyId, int storyRelayId)
    {
        Hash storyRelay = StoryRelayData.GetByStoryRelayId(storyId, storyRelayId);
        if (storyRelay.ToInt("id") == 0)
        {
            return new Hash((int)CodeType.StoryRelayNotExists, "故事续作不存在");
        }
        if (storyRelay.ToInt("clientId") == token.ToInt("clientId"))
        {
            StoryRelayData.Delete(token.ToInt("clientId"), storyId, storyRelayId);

            StoryData.Reset(token.ToInt("clientId"), storyId);
            return new Hash((int)CodeType.OK, "成功");
        }
        return new Hash((int)CodeType.StoryRelayNotAuthor, "故事续作只有作者本人可删除");
    }
}