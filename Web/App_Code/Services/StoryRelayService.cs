using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 故事续作业务操作类
/// </summary>
public class StoryRelayService
{
    /// <summary>
    /// 获取故事列表
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续作编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash All(Hash token, int storyId, int storyRelayId)
    {
        Hash data = new Hash();
        HashCollection items = new HashCollection();
        HashCollection relays = StoryRelayData.All(token.ToInt("clientId"), storyId).ToHashCollection("data");
        HashCollection parents = StoryRelayService.GetParent(relays, storyRelayId);
        HashCollection childs = StoryRelayService.GetChild(relays, storyRelayId);

        items.AddRange(parents);
        items.AddRange(childs);

        data["data"] = items;
        return new Hash((int)CodeType.OK, "成功", data);
    }
    /// <summary>
    /// 获取指定分支后的所有续作
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="storyRelayId">int 续作编号</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Branch(Hash token, int storyId, int storyRelayId)
    {
        Hash data = new Hash();
        HashCollection items = new HashCollection();
        HashCollection relays = StoryRelayData.All(token.ToInt("clientId"), storyId).ToHashCollection("data");
        HashCollection current = StoryRelayService.GetCurrent(relays, storyRelayId);
        HashCollection childs = StoryRelayService.GetChild(relays, storyRelayId);

        items.AddRange(current);
        items.AddRange(childs);

        data["data"] = items;
        return new Hash((int)CodeType.OK, "成功", data);
    }
    /// <summary>
    /// 续写故事
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="storyId">int 故事编号</param>
    /// <param name="parentStoryRelayId">int 父续作编号</param>
    /// <param name="content">string 内容</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Create(Hash token, int storyId, int parentStoryRelayId, string content)
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
        Hash storyRelay = StoryRelayData.GetByStoryRelayId(storyId, parentStoryRelayId);
        if (storyRelay.ToInt("id") == 0)
        {
            return new Hash((int)CodeType.StoryRelayStoryCountLess, "你要续写的分支不存在");
        }
        int storyRelayId = StoryRelayData.NewStoryRelayId(storyId);
        if (StoryRelayData.Create(token.ToInt("clientId"), storyId, storyRelayId, parentStoryRelayId, content, storyRelay.ToInt("depth") + 1) > 0)
        {
            //  更新统计
            StoryRelayData.Recount(token.ToInt("clientId"), storyId, storyRelayId, parentStoryRelayId);

            //  发送通知
            ClientNotifyData.Bookmark(token.ToInt("clientId"), storyId, storyRelayId);
            return new Hash((int)CodeType.OK, "成功", StoryRelayData.GetByStoryRelayId(storyId, storyRelayId));
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败，story relay");
    }

    #region 辅助方法
    private static HashCollection GetCurrent(HashCollection items, int storyRelayId)
    {
        HashCollection result = new HashCollection();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ToInt("storyRelayId") == storyRelayId)
            {
                result.Add(items[i]);
                result[0]["previousStoryRelayId"] = (i > 0 && items[i - 1].ToInt("parentStoryRelayId") == items[i].ToInt("parentStoryRelayId") ? items[i - 1].ToInt("storyRelayId") : 0);
                result[0]["nextStoryRelayId"] = (i < items.Count - 1 && items[i + 1].ToInt("parentStoryRelayId") == items[i].ToInt("parentStoryRelayId") ? items[i + 1].ToInt("storyRelayId") : 0);
                break;
            }
        }

        return result;
    }
    /// <summary>
    /// 获取所有父节点
    /// </summary>
    /// <param name="all">HashCollection 所有节点</param>
    /// <param name="storyRelayId">int 父节点编号</param>
    /// <returns>HashCollection 节点集合</returns>
    private static HashCollection GetParent(HashCollection items, int storyRelayId)
    {
        HashCollection result = new HashCollection();
        
        int times = 0;
        bool matched = true;
        while (matched == true && times < 100)
        {
            matched = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ToInt("storyRelayId") == storyRelayId)
                {
                    matched = true;
                    result.Insert(0, items[i]);
                    result[0]["previousStoryRelayId"] = (i > 0 && items[i - 1].ToInt("parentStoryRelayId") == items[i].ToInt("parentStoryRelayId") ? items[i - 1].ToInt("storyRelayId") : 0);
                    result[0]["nextStoryRelayId"] = (i < items.Count - 1 && items[i + 1].ToInt("parentStoryRelayId") == items[i].ToInt("parentStoryRelayId") ? items[i + 1].ToInt("storyRelayId") : 0);
                    storyRelayId = items[i].ToInt("parentStoryRelayId");
                    break;
                }
            }
            storyRelayId = (matched == false ? 0 : storyRelayId);
            times++;
        }

        return result;
    }
    /// <summary>
    /// 获取所有子节点
    /// </summary>
    /// <param name="all">HashCollection 所有节点</param>
    /// <param name="storyRelayId">int 父节点编号</param>
    /// <returns>HashCollection 节点集合</returns>
    private static HashCollection GetChild(HashCollection items, int storyRelayId)
    {
        HashCollection result = new HashCollection();

        //  读取所有子续作
        int times = 0;
        bool matched = true;
        while (matched == true && times < 100)
        {
            matched = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ToInt("parentStoryRelayId") == storyRelayId)
                {
                    matched = true;
                    result.Add(items[i]);
                    result[result.Count - 1]["previousStoryRelayId"] = ( i > 0 && items[i - 1].ToInt("parentStoryRelayId") == storyRelayId ?  items[i - 1].ToInt("storyRelayId") : 0);
                    result[result.Count - 1]["nextStoryRelayId"] = (i < items.Count - 1 && items[i + 1].ToInt("parentStoryRelayId") == storyRelayId ? items[i + 1].ToInt("storyRelayId") : 0);
                    storyRelayId = items[i].ToInt("storyRelayId");
                    break;
                }
            }
            storyRelayId = (matched == false ? 0 : storyRelayId);
            times++;
        }

        return result;
    }
    #endregion
}