using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端故事业务操作类
/// </summary>
public class ClientStoryService
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
        return new Hash((int)CodeType.OK, "成功", StoryData.List(token.ToInt("clientId"), pageId, pageSize));
    }
    /// <summary>
    /// 删除故事
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Delete(Hash token, int storyId)
    {
        Hash story = StoryData.GetByStoryId(storyId);
        if (story.ToInt("storyId") == 0)
        {
            return new Hash((int)CodeType.StoryNotExists, "不是不存在");
        }
        if (story.ToInt("clientId") == token.ToInt("clientId"))
        {
            StoryData.Delete(token.ToInt("clientId"), storyId);
            return new Hash((int)CodeType.OK, "成功");
        }
        return new Hash((int)CodeType.StoryNotAuthor, "故事只有作者本人可删除");
    }
}