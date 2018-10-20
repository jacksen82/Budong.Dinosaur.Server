using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 游戏业务操作类
/// </summary>
public class GameService
{
    /// <summary>
    /// 获取排行榜信息
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <returns>Hash 排行榜</returns>
    public static Hash Rank(Hash token)
    {
        Hash data = new Hash();
        HashCollection all = ClientData.GetRank(token.ToInt("clientId")).ToHashCollection("data");
        HashCollection global = new HashCollection();
        HashCollection friend = new HashCollection();
        for (int i = 0; i < all.Count; i++)
        {
            if (i < 10)
            {
                global.Add(all[i]);
            }
            if (all[i].ToInt("relateType") > 0 || all[i].ToInt("clientId") == token.ToInt("clientId"))
            {
                friend.Add(all[i]);
            }
        }
        data["global"] = global;
        data["friend"] = friend;
        return new Hash((int)CodeType.OK, "成功", data);
    }
    /// <summary>
    /// 获取下一个题目
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <returns>Hash 结果信息</returns>
    public static Hash Assign(Hash token)
    {
        Random random = new Random();
        Hash data = ClientDinosaurData.Assign(token.ToInt("clientId"));
        HashCollection names = DinosaurData.AllName(0).ToHashCollection("data");

        data["length"] = names.Count;
        data["duration"] = token.ToInt("duration");
        data["score"] = token.ToInt("score");
        data["cards"] = token.ToInt("cards");
        data["status"] = token.ToInt("status");

        int indexAt = random.Next(0, 4);
        if (data.ToInt("dinosaurId")> 0)
        {
            HashCollection options = new HashCollection();

            //  剔除当前题目
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i].ToString("name") == data.ToString("name"))
                {
                    names.RemoveAt(i);
                }
            }
            
            //  添加选项
            for (int i = 0; i < 4; i++)
            {
                if (i == indexAt)
                {
                    options.Add(new Hash("{\"name\":\"" + data.ToString("name") + "\"}"));
                }
                else
                {
                    int index = random.Next(0, names.Count);
                    options.Add(names[i]);
                    names.RemoveAt(index);
                }
            }

            data["options"] = options;

            return new Hash((int)CodeType.OK, "成功", data);
        }
        return new Hash((int)CodeType.OK, "所有题目都答完了", data);
    }
    /// <summary>
    /// 继续答题
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <returns>Hash 结果信息</returns>
    public static Hash Continue(Hash token)
    {
        if (token.ToInt("status") == 200)
        {
            if (token.ToInt("cards") > 0)
            {
                if (ClientData.Continue(token.ToInt("clientId")) > 0)
                {
                    return new Hash((int)CodeType.OK, "成功", ClientData.GetByClientId(token.ToInt("clientId")));
                }
                return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
            }
            return new Hash((int)CodeType.ClientHaveNotCard, "没有可用复活卡");
        }
        return new Hash((int)CodeType.OK, "成功", ClientData.GetByClientId(token.ToInt("clientId")));
    }
    /// <summary>
    /// 重新开始
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <returns>Hash 结果信息</returns>
    public static Hash Restart(Hash token)
    {
        ClientData.Restart(token.ToInt("clientId"));
        ClientDinosaurData.Restart(token.ToInt("clientId"));

        return new Hash((int)CodeType.OK, "成功", ClientData.GetByClientId(token.ToInt("clientId")));
    }
    /// <summary>
    /// 答题结果
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="dinosaurId">int 题目编号</param>
    /// <param name="result">int 答题结果</param>
    /// <returns>Hash 结果信息</returns>
    public static Hash Answer(Hash token, int dinosaurId, int result)
    {
        if (ClientDinosaurData.Answer(token.ToInt("clientId"), dinosaurId, result) > 0)
        {
            ClientData.Answer(token.ToInt("clientId"), result);
            Hash data = ClientData.GetByClientId(token.ToInt("clientId"));
            Hash rank = ClientData.GetRank(data.ToInt("clientId"), data.ToInt("score"));
            data["position"] = rank.ToFloat("position");
            return new Hash((int)CodeType.OK, "成功", data);
        }
        return new Hash((int)CodeType.DataBaseUnknonw, "数据库操作失败");
    }
}