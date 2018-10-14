using System;
using System.Text;
using System.Web;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 基础类型判断
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// 判断指定对象是否为空
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="type">Type 类型</param>
        /// <returns>bool 是否为空，true 为空</returns>
        public static bool IsNull(object obj, Type type = null)
        {
            if (obj == null) return true;
            else
            {
                switch (obj.GetType().Name)
                {
                    case "String": return obj.ToString().Trim() == "";
                    case "DBNull": return true;
                    default: return false;
                }
            }
        }
        /// <summary>
        /// 判断指定对象是否为指定类型
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="type">string 类型</param>
        /// <returns>bool 是否指定类型，true 是</returns>
        public static bool IsType(object obj, string type)
        {
            if (obj == null) return false;
            else
            {
                if (obj.GetType().Name.Equals(type, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 判断两个对象是否一致
        /// </summary>
        /// <param name="obj">object 目标对象</param>
        /// <param name="value">object 比较对象</param>
        /// <param name="rigorous">bool 是否严谨模式</param>
        /// <returns>bool 是否一致</returns>
        public static bool IsEquals(object obj, object value, bool rigorous = false)
        {
            if (rigorous == true)
            {
                return obj == value;
            }
            else
            {
                return Convert.ToString(obj).ToLower() == Convert.ToString(value).ToLower();
            }
        }
        /// <summary>
        /// 判断指定数字是否为手机号码
        /// </summary>
        /// <param name="obj">long 数字</param>
        /// <returns>bool 是否合法</returns>
        public static bool IsMobile(long obj)
        {
            if (obj > 10000000000 && obj < 19900000000)
            {
                int[] prefix = { 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 147, 150, 151, 152, 153, 155, 156, 157, 158, 159, 180, 187, 188, 185, 186, 189 };
                if (Array.IndexOf(prefix, (obj / 100000000)) > -1)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取指定字符串的长度 （汉字占两个字节）
        /// </summary>
        /// <param name="str">string 字符串</param>
        /// <returns>int 返回字符串的长度</returns>
        public static int Length(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="length">int 长度</param>
        /// <param name="mode">int 模式</param>
        /// <returns>string 随机数</returns>
        public static string GetRandom(int length, int mode = 0)
        {
            string result = String.Empty;
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharArray;
            Random random = new Random();
            if (mode == 101)
            {
                allChar = "0,1,2,3,4,5,6,7,8,9";
            }
            allCharArray = allChar.Split(',');
            for (int i = 0; i < length; i++)
            {
                result += allCharArray[random.Next(allCharArray.Length)];
            }
            return result;
        }
        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        /// <returns>string IP 地址</returns>
        public static string GetAddress()
        {
            if (String.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
        }
        /// <summary>
        /// 获取微信时间戳
        /// </summary>
        /// <param name="datetime">DateTime 时间</param>
        /// <returns>string 时间戳</returns>
        public static string GetTimeStamp(DateTime datetime)
        {
            TimeSpan timeStamp = datetime - DateTime.Parse("1970-01-01");
            return Math.Floor(timeStamp.TotalSeconds).ToString();
        }
    }
}
