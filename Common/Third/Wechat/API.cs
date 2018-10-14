using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Budong.Common.Third.Wechat
{
    /// <summary>
    /// 微信 API 封装
    /// </summary>
    public class API
    {
        /// <summary>
        /// 通过 Code 获取用户 Session 信息
        /// </summary>
        /// <param name="appId">string APP 标识</param>
        /// <param name="appSecret">string APP 秘钥</param>
        /// <param name="code">string 登录验证票</param>
        /// <returns>Hash Session 信息</returns>
        public static Utils.Hash Code2Session(string appId, string appSecret, string code)
        {
            try
            {
                WebRequest request = WebRequest.Create("https://api.weixin.qq.com/sns/jscode2session"
                    + "?appid=" + appId
                    + "&secret=" + appSecret
                    + "&js_code=" + code
                    + "&grant_type=authorization_code");
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8")))
                {
                    return new Utils.Hash(reader.ReadToEnd());
                }
            }
            catch (System.Exception ex)
            {
                return new Utils.Hash(9999, ex.ToString());
            }
        }
        /// <summary>
        /// 获取解密数据
        /// </summary>
        /// <param name="encryptedData">string 加密信息</param>
        /// <param name="sessionKey">string 密钥</param>
        /// <param name="iv">string 向量</param>
        /// <returns>Hash 用户信息</returns>
        public static Utils.Hash GetEncryptedData(string encryptedData, string sessionKey, string iv)
        {
            if (!Utils.Genre.IsNull(encryptedData) &&
                !Utils.Genre.IsNull(sessionKey) &&
                !Utils.Genre.IsNull(iv))
            {
                return new Utils.Hash(AES.Decrypt(encryptedData, sessionKey, iv));
            }
            return new Utils.Hash();
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="appKey">APP 标识</param>
        /// <param name="appSecret">APP 秘钥</param>
        /// <returns>Hash 操作结果</returns>
        public static Utils.Hash GetAccessToken(string appKey, string appSecret) 
        {
            string jsonResult = API.Get("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appKey + "&secret=" + appSecret);
            return new Utils.Hash(jsonResult);
        }
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="accessToken">string AccessToken 凭证</param>
        /// <param name="data">string 参数 [ { "scene":"","page":"page/index/index","width":"340" } ]</param>
        /// <param name="buffer">byte[] 数据流</param>
        /// <returns>Hash 操作结果</returns>
        public static Utils.Hash GetQrCode(string accessToken, string data, out byte[] buffer)
        {
            string jsonResult = API.PostData("https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=" + accessToken, data, out buffer);
            return new Utils.Hash(jsonResult);
        }
        /// <summary>
        /// 发送客服消息
        /// </summary>
        /// <param name="accessToken">string AccessToken 凭证</param>
        /// <param name="data">string 参数 [ { "touser":"OPENID", "msgtype":"text", "text": { "content":"Hello World" } } ]</param>
        /// <returns>Hash 操作结果</returns>
        public static Utils.Hash Send(string accessToken, string data)
        {
            string jsonResult = API.PostData("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + accessToken, data);
            return new Utils.Hash(jsonResult);
        }
        /// <summary>
        /// 获取支付信息
        /// </summary>
        /// <param name="appId">string APP 标识</param>
        /// <param name="appSecret">string APP 秘钥</param>
        /// <param name="merchantId">string 商户号</param>
        /// <param name="merchantKey">string 商户秘钥</param>
        /// <param name="openId">string 用户标识</param>
        /// <param name="orderId">string 订单编号</param>
        /// <param name="body">string 商品描述</param>
        /// <param name="price">int 价格</param>
        /// <returns>Hash 操作结果</returns>
        public static Utils.Hash Prepay(string appId, string appSecret, string merchantId, string merchantKey, string openId, string orderId, string body, int price)
        {
            Utils.Hash queries = new Utils.Hash();
            queries["appid"] = appId;
            queries["mch_id"] = merchantId;
            queries["nonce_str"] = Utils.Genre.GetRandom(20);
            queries["body"] = body;
            queries["out_trade_no"] = orderId;
            queries["total_fee"] = price;
            queries["spbill_create_ip"] = Utils.Genre.GetAddress();
            queries["notify_url"] = "https://shenxu.name/capsule/api/third/wechat/pay/callback.ashx";
            queries["trade_type"] = "JSAPI";
            queries["openid"] = openId;
            queries["sign"] = Signature(merchantKey, queries);
            string xmlResult = API.PostPay("https://api.mch.weixin.qq.com/pay/unifiedorder", queries);
            return Utils.Hash.FromXML(xmlResult);
        }
        /// <summary>
        /// 获取调起支付的参数 
        /// </summary>
        /// <param name="appId">string APP 标识</param>
        /// <param name="merchantId">string 商户号</param>
        /// <param name="merchantKey">string 商户秘钥</param>
        /// <param name="prepayId">string 支付编号</param>
        /// <returns>Hash 操作结果</returns>
        public static Utils.Hash Payment(string appId, string merchantId, string merchantKey, string prepayId)
        {
            Utils.Hash queries = new Utils.Hash();
            queries["appId"] = appId;
            queries["timeStamp"] = Utils.Genre.GetTimeStamp(DateTime.Now);
            queries["nonceStr"] = Utils.Genre.GetRandom(20);
            queries["package"] = "prepay_id=" + prepayId;
            queries["signType"] = "MD5";
            queries["paySign"] = Signature(merchantKey, queries);
            return queries;
        }

        #region 私有方法
        /// <summary>
        /// 请求微信接口
        /// </summary>
        /// <param name="url">string 接口地址</param>
        /// <returns>string 返回结果</returns>
        public static string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "textml;charset=UTF-8";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
        /// <summary>
        /// 请求微信接口
        /// </summary>
        /// <param name="url">string 接口地址</param>
        /// <param name="data">string 接口参数</param>
        /// <returns>string 返回结果</returns>
        public static string PostData(string url, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.ContentLength = byteData.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteData, 0, byteData.Length);
                requestStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 请求微信接口
        /// </summary>
        /// <param name="url">string 接口地址</param>
        /// <param name="data">string 接口参数</param>
        /// <param name="buffer">byte[] 数据流</param>
        /// <returns>string 返回结果</returns>
        public static string PostData(string url, string data, out byte[] buffer)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.ContentLength = byteData.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteData, 0, byteData.Length);
                requestStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        List<byte> bytes = new List<byte>();
                        int _byte = responseStream.ReadByte();
                        while (_byte != -1)
                        {
                            bytes.Add((byte)_byte);
                            _byte = responseStream.ReadByte();
                        }
                        if (bytes.Count > 1024)
                        {
                            buffer = bytes.ToArray();
                            return "{\"code\":0, \"message\":\"成功\"}";
                        }
                        buffer = new byte[0];
                        return Encoding.UTF8.GetString(bytes.ToArray());
                    }
                }
            }
        }
        /// <summary>
        /// 请求微信支付接口
        /// </summary>
        /// <param name="url">string 接口地址</param>
        /// <param name="data">Utils.Hash 接口参数</param>
        /// <returns>string 返回结果</returns>
        public static string PostPay(string url, Utils.Hash data)
        {
            Encoding encode = Encoding.GetEncoding("UTF-8");
            CookieCollection cookies = new CookieCollection();
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            StringBuilder dataBuffer = new StringBuilder();
            dataBuffer.AppendFormat(data.ToXML());
            byte[] dataBytes = encode.GetBytes(dataBuffer.ToString());
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader responseReader = new StreamReader(responseStream);
                    return responseReader.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// 字符串签名
        /// </summary>
        /// <param name="merchantKey">string 商户秘钥</param>
        /// <param name="queries">string 参数集合</param>
        /// <returns>string 签名结果</returns>
        public static string Signature(string merchantKey, Utils.Hash queries)
        {
            string queryString = String.Empty;
            ArrayList allKeys = new ArrayList(queries.Keys);
            allKeys.Sort();
            foreach (string key in allKeys)
            {
                if (key != "sign" && key != "paySign")
                {
                    queryString += String.IsNullOrEmpty(queryString) ? "" : "&";
                    queryString += key + "=" + queries.ToString(key);
                }
            }
            queryString += "&key=" + merchantKey;
            return Secret.MD5.Encode(queryString);
        }
        /// <summary>
        /// 判断验证结果
        /// </summary>
        /// <param name="sender">object 发送对象</param>
        /// <param name="certificate">X509Certificate 验证方法</param>
        /// <param name="chain">X509Chain 约束</param>
        /// <param name="errors">SslPolicyErrors 错误</param>
        /// <returns>bool 有效性</returns>
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion
    }
}
