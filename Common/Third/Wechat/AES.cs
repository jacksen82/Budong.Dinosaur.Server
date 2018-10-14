using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Budong.Common.Third.Wechat
{
    /// <summary>  
    /// 微信数据解密
    /// </summary>  
    public class AES
    {
        /// <summary> 
        /// AES 解密字符串 
        /// </summary> 
        /// <param name="data">string 输入的加密内容</param> 
        /// <param name="key">string 密钥</param> 
        /// <param name="iv">string 向量</param> 
        /// <returns name="result">解密后的字符串</returns> 
        public static string Decrypt(string data, string key, string iv)
        {
            try
            {
                iv = iv.Replace(" ", "+");
                key = key.Replace(" ", "+");
                data = data.Replace(" ", "+");

                byte[] encryptedData = Convert.FromBase64String(data);

                RijndaelManaged rijndaelCipher = new RijndaelManaged();

                rijndaelCipher.Key = Convert.FromBase64String(key);
                rijndaelCipher.IV = Convert.FromBase64String(iv);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;

                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                return Encoding.UTF8.GetString(plainText);
            }
            catch (Exception e)
            {
                return "{ \"code\":10000, \"message\": \""+ e.ToString() +"\" }";
            }
        }
    }
}