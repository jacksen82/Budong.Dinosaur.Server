using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 字符集操作类
    /// </summary>
    public class Charset
    {
        /// <summary>
        /// 截取指定长度的字符串 （汉字占两个字节）
        /// </summary>
        /// <param name="str">string 字符串</param>
        /// <param name="len">long 长度</param>
        /// <param name="def">string 超出后显示的文字</param>
        /// <returns>string 返回截取后的字符串</returns>
        public static string SubString(string str, int len, string def = "..")
        {
            int clen = 0;
            int elen = 0;
            byte[] sbytes = Encoding.Default.GetBytes(str);
            if (len < sbytes.Length)
            {
                for (int i = 0; i < (len - 2); i++)
                {
                    if (sbytes[i] > 127) clen++;
                    elen++;
                }
                if (Math.IEEERemainder(clen, 2) != 0) elen--;
                byte[] rbytes = new byte[elen];
                Array.Copy(sbytes, rbytes, elen);
                return Encoding.Default.GetString(rbytes) + def;
            }
            else
            {
                return str;
            }
        }
        /// <summary>
        /// 过滤句子中的无用字符
        /// </summary>
        /// <param name="str">string 句子</param>
        /// <param name="def">string 默认待过滤字符</param>
        /// <returns>string 返回结果</returns>
        public static string Clean(string str, string def = ".,:;!?。，：；！？<>(){}''\"《》（）｛｝【】‘’“”")
        {
            str = Convert.ToString(str);
            str = Regex.Replace(str, "　", " ");
            str = Regex.Replace(str, "(\\s+)", " ");
            str = Regex.Replace(str, "\\s*([" + def + "])\\s*", "$1");
            return str;
        }
    }
}
