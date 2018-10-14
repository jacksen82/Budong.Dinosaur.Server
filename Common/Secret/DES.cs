using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Budong.Common.Secret
{
    /// <summary>
    /// DES 加密解密类
    /// </summary>
    public class DES
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="text">string 要加密的字符串</param>
        /// <param name="key">string 加密密钥</param>
        /// <returns>string 加密后的字符串</returns>
        public static string Encode(string text, string key = "19820116")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="text">string 要解密的字符串</param>
        /// <param name="key">string 解密密钥</param>
        /// <returns>string 解密后的字符串</returns>
        public static string Decode(string text, string key = "19820116")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[text.Length / 2];
            for (int x = 0; x < text.Length / 2; x++)
            {
                int i = (Convert.ToInt32(text.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}
