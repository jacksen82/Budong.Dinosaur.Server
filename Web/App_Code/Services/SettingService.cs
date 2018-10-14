using System;
using System.Web;
using Budong.Common.Utils;
using Budong.Common.Third.Wechat;

/// <summary>
/// 小程序应用业务操作类
/// </summary>
public class SettingsService
{
    /// <summary>
    /// 获取小程序访问 access_token
    /// </summary>
    /// <returns>string access_token</returns>
    public static string AccessToken()
    {
        Hash setting = SettingData.Detail();
        if (setting.IsNull("accessToken") || setting.ToDateTime("accessTokenExpireIn") < DateTime.Now)
        {
            Hash accessToken = API.GetAccessToken(setting.ToString("appKey"), setting.ToString("appSecret"));
            SettingData.SetItem("accessToken", accessToken.ToString("access_token"));
            SettingData.SetItem("accessTokenExpireIn", DateTime.Now.AddSeconds(accessToken.ToInt("expires_in")).ToString());
            return accessToken.ToString("access_token");
        }
        return setting.ToString("accessToken");
    }
}