using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// ClientNotifyService 的摘要说明
/// </summary>
public class ClientNotifyService
{
    /// <summary>
    /// 获取消息动态
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="pageId">int 页码</param>
    /// <param name="pageSize">int 页尺寸</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash List(Hash token, int pageId, int pageSize)
    {
        Hash data = ClientNotifyData.List(token.ToInt("clientId"), pageId, pageSize);
        ClientNotifyData.ReadAll(token.ToInt("clientId"));
        return new Hash((int)CodeType.OK, "成功", data);
    }
}