using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 故事业务操作类
/// </summary>
public class StoryService
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
        return new Hash((int)CodeType.OK,"成功", StoryData.List(pageId, pageSize));
    }
    /// <summary>
    /// 创建新故事
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="content">string 内容</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Create(Hash token, string content)
    {
        if (Genre.IsNull(content))
        {
            return new Hash((int)CodeType.StoryContentRequired, "故事内容不能为空");
        }
        if (token.ToInt("clientId") == 0)
        {
            return new Hash((int)CodeType.ClientNotExists, "客户端不存在");
        }
        if (token.ToInt("actived") != 1)
        {
            return new Hash((int)CodeType.ClientInaction, "客户端未激活");
        }
        if (token.ToInt("disabled") != 0)
        {
            return new Hash((int)CodeType.ClientDisabled, "客户端被禁用");
        }
        if (token.ToInt("relayCount") < 3)
        {
            return new Hash((int)CodeType.StoryRelayStoryCountLess, "续写过 3 个以上故事，才能开启新故事");
        }

        int storyId = StoryData.NewStoryId();
        int storyRelayId = StoryRelayData.NewStoryRelayId(storyId);
        
        if (StoryData.Create(token.ToInt("clientId"), storyId) > 0)
        {
            if (StoryRelayData.Create(token.ToInt("clientId"), storyId, storyRelayId, 0, content, 1) > 0)
            {
                StoryRelayData.Recount(token.ToInt("clientId"), storyId, storyRelayId, 0);

                return new Hash((int)CodeType.OK, "成功", StoryData.GetByStoryId(storyId));
            }
            return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败，create story relay");
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败，create story");
    }
}