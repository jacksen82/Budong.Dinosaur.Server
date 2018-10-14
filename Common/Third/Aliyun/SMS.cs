using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;

using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace Budong.Common.Third.Aliapi
{
    /// <summary>
    /// 短信发送类
    /// </summary>
    public class SMS
    {
        /// <summary>
        /// 发送短信内容
        /// </summary>
        /// <param name="mobiles">string[] 手机号码</param>
        /// <param name="templateCode">string 模板编号</param>
        /// <param name="templateData">Hash 模板数据</param>
        /// <returns>Hash 发送结果</returns>
        public static Utils.Hash Send(string[] mobiles, string templateCode, Utils.Hash templateData)
        {
            String product = "Dysmsapi";
            String domain = "dysmsapi.aliyuncs.com";
            String accessId = "LTAIOAQK08HWi1vJ";
            String accessSecret = "WbyukU2Y8h1FOJMDMi1J8da1xFS0L8";
            String regionIdForPop = "cn-hangzhou";
            
            IClientProfile profile = DefaultProfile.GetProfile(regionIdForPop, accessId, accessSecret);
            DefaultProfile.AddEndpoint(regionIdForPop, regionIdForPop, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                request.PhoneNumbers = String.Join(",", mobiles);
                request.SignName = "时光胶囊";
                request.TemplateCode = templateCode;
                request.TemplateParam = templateData.ToJSON();
                request.OutId = DateTime.Now.Ticks.ToString();
                request.AcceptFormat = Aliyun.Acs.Core.Http.FormatType.JSON;

                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);

                if (sendSmsResponse.Code == "OK")
                {
                    return new Utils.Hash(0, "成功");
                }
                return new Utils.Hash(9001, sendSmsResponse.Message);

            }
            catch (ServerException e)
            {
                return new Utils.Hash(9001, e.ToString());
            }
            catch (ClientException e)
            {
                return new Utils.Hash(9001, e.ToString());
            }
        }
    }
}
