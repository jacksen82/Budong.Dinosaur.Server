using System;
using System.Text;
using System.Security.Cryptography;

namespace Budong.Common.Secret
{
    /// <summary>
    /// MD5 加密类
    /// </summary>
    public class MD5
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="text">string 要加密的字符串</param>
        /// <returns>string 加密结果</returns>
        public static string Encode(string text)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
    }
}
