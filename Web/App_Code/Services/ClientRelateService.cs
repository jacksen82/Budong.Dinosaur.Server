using System;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 客户端关系业务操作类
/// </summary>
public class ClientRelateService
{
    /// <summary>
    /// 建立关系
    /// </summary>
    /// <param name="token">Hash 客户端信息</param>
    /// <param name="fromClientId">int 邀请人客户端编号</param>
    /// <param name="scene">int 来源场景</param>
    /// <param name="encryptedData">string 群标识加密信息</param>
    /// <param name="iv">string 群标识加密向量</param>
    /// <returns>Hash 返回结果</returns>
    public static Hash Create(Hash token, int fromClientId, int scene, string encryptedData, string iv)
    {
        if (fromClientId == 0)
        {
            return new Hash((int)CodeType.ClientRelateRequired, "邀请人不存在");
        }
        Hash shareTicket = new Hash();
        if (!Genre.IsNull(encryptedData) && !Genre.IsNull(iv) && !token.IsNull("sessionKey"))
        {
            ClientLogService.Append(encryptedData + " | " + iv + " | " + token.ToString("sessionKey"));
            shareTicket = API.GetEncryptedData(encryptedData, token.ToString("sessionKey"), iv);
        }
        if (shareTicket.IsNull("openGId"))
        {
            if (token.ToInt("clientId") != fromClientId)
            {
                ClientRelateData.Create(token.ToInt("clientId"), fromClientId, RelateType.FromFriend);
                ClientRelateData.Create(fromClientId, token.ToInt("clientId"), RelateType.FromFriend);
            }
        }
        else
        {
            ClientGroupData.Create(token.ToInt("clientId"), shareTicket.ToString("openGId"));
            ClientGroupData.Renovate(shareTicket.ToString("openGId"));
        }
        return new Hash((int)CodeType.OK, "成功", shareTicket);
    }
}