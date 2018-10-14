using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端关注业务操作类
/// </summary>
public class ClientBookmarkService
{
    /// <summary>
    /// 获取关注故事集合
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash List(Hash token, int pageId, int pageSize)
    {
        return new Hash((int)CodeType.OK, "成功", ClientBookmarkData.List(token.ToInt("clientId"), pageId, pageSize));
    }
    /// <summary>
    /// 关注故事
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续作编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Bookmark(Hash token, int storyId, int storyRelayId)
    {
        Hash data = new Hash();
        Hash bookmark = ClientBookmarkData.GetByStoryRelayId(token.ToInt("clientId"), storyId, storyRelayId);
        if (bookmark.ToInt("id") > 0)
        {
            if (ClientBookmarkData.Delete(token.ToInt("clientId"), storyId, storyRelayId) > 0)
            {
                data["action"] = 201;
                ClientBookmarkData.Recount(token.ToInt("clientId"), storyId, storyRelayId);
                return new Hash((int)CodeType.OK, "成功", data);
            }
            return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败，delete");
        }
        else
        {
            if (ClientBookmarkData.Create(token.ToInt("clientId"), storyId, storyRelayId) > 0)
            {
                data["action"] = 101;
                ClientBookmarkData.Recount(token.ToInt("clientId"), storyId, storyRelayId);

                //  发送通知
                ClientNotifyData.Bookmark(token.ToInt("clientId"), storyId, storyRelayId);
                return new Hash((int)CodeType.OK, "成功", data);
            }
            return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败，create");
        }
    }
    /// <summary>
    /// 取消关注
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 故事续写编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Delete(Hash token, int storyId, int storyRelayId)
    {
        if (ClientBookmarkData.Delete(token.ToInt("clientId"), storyId, storyRelayId) > 0)
        {
            return new Hash((int)CodeType.OK, "成功");
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
    }
}