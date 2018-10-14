using System;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 类型转换类
    /// </summary>
    public class Parse
    {
        /// <summary>
        /// 转换指定对象为整形数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">int 默认值</param>
        /// <returns>int 返回转换后的整形数值</returns>
        public static int ToInt(object obj, int def = 0)
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                try
                {
                    return Int32.Parse(obj.ToString());
                }
                catch
                {
                    return def;
                }
            }
        }
        /// <summary>
        /// 转换指定对象为长整形数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">int 默认值</param>
        /// <returns>int 返回转换后的长整形数值</returns>
        public static long ToLong(object obj, long def = 0)
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                try
                {
                    return long.Parse(obj.ToString());
                }
                catch
                {
                    return def;
                }
            }
        }
        /// <summary>
        /// 转换指定对象为浮点数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">float 默认值</param>
        /// <returns>float 返回转换后的浮点数值</returns>
        public static float ToFloat(object obj, float def = 0)
        {
            return Parse.ToFloat(obj, 0, def);
        }
        /// <summary>
        /// 转换指定对象为浮点数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="digits">int 精度</param>
        /// <param name="def">float 默认值</param>
        /// <returns>float 返回转换后的浮点数值</returns>
        public static float ToFloat(object obj, int digits, float def = 0)
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                try
                {
                    return float.Parse(Math.Round(double.Parse(obj.ToString()), digits).ToString());
                }
                catch
                {
                    return def;
                }
            }
        }
        /// <summary>
        /// 转换指定对象为双精度数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">double 默认值</param>
        /// <returns>int 返回转换后的双精度数值</returns>
        public static double ToDouble(object obj, double def = 0)
        {
            return Parse.ToDouble(obj, 0, def);
        }
        /// <summary>
        /// 转换指定对象为双精度数值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="digits">int 精度</param>
        /// <param name="def">double 默认值</param>
        /// <returns>int 返回转换后的双精度数值</returns>
        public static double ToDouble(object obj, int digits, double def = 0)
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                try
                {
                    return Math.Round(double.Parse(obj.ToString()), digits);
                }
                catch
                {
                    return def;
                }
            }
        }
        /// <summary>
        /// 转换指定对象为字符串
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">string 默认值</param>
        /// <returns>string 返回转换后的字符串</returns>
        public static string ToString(object obj, string def = "")
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                return obj.ToString();
            }
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="datetime">DateTime 时间</param>
        /// <returns>long 时间戳</returns>
        public static long ToTimeStamp(DateTime datetime)
        {
            DateTime initial = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return System.Convert.ToInt64(Math.Floor((DateTime.Now - initial).TotalMilliseconds));
        }
        /// <summary>
        /// 转换长整形数值为日期时间值 ( 微信可用 )
        /// </summary>
        /// <param name="timeStamp">long 时间戳</param>
        /// <returns>DateTime 返回转换后的日期时间值</returns>
        public static DateTime ToTimeStamp(long timeStamp)
        {
            DateTime initial = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan nowspan = new TimeSpan(timeStamp * 10000);
            return initial.Add(nowspan);
        }
        /// <summary>
        /// 转换指定对象为日期时间值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <returns>string 返回转换后的日期时间值</returns>
        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, DateTime.Now);
        }
        /// <summary>
        /// 转换指定对象为日期时间值
        /// </summary>
        /// <param name="obj">object 对象</param>
        /// <param name="def">DateTime 默认值</param>
        /// <returns>string 返回转换后的日期时间值</returns>
        public static DateTime ToDateTime(object obj, DateTime def)
        {
            if (Genre.IsNull(obj))
            {
                return def;
            }
            else
            {
                try
                {
                    return DateTime.Parse(obj.ToString());
                }
                catch
                {
                    return def;
                }
            }
        }
        /// <summary>
        /// 专制指定日期时间为描述文本
        /// </summary>
        /// <param name="second">int 秒数</param>
        /// <returns>string 描述文本</returns>
        public static string ToTimeTip(int second)
        {
            if (second > 60)
            {
                double minute = Math.Floor(second / 60.0);
                if (second > 60 * 60)
                {
                    double hour = Math.Floor(second / 60.0 / 60.0);
                    if (second > 60 * 60)
                    {
                        double day = Math.Floor(second / 24.0 / 60.0 / 60.0);
                        return day + "天" + (hour % 24) + "小时" + (minute % 60) + "分" + (second % 60) + "秒";
                    }
                    else
                    {
                        return hour + "小时" + (minute % 60) + "分" + (second % 60) + "秒";
                    }
                }
                else
                {
                    return minute + "分" + (second % 60) + "秒";
                }
            }
            else
            {
                return second + "秒";
            }
        }
        /// <summary>
        /// 专制指定日期时间为描述文本
        /// </summary>
        /// <param name="datetime">DateTime 时间</param>
        /// <returns>string 描述文本</returns>
        public static string ToTimeTip(DateTime datetime)
        {
            TimeSpan span = DateTime.Now - datetime;
            double intdays = Math.Floor(span.TotalDays);
            double inthours = Math.Floor(span.TotalHours);
            double intminutes = Math.Floor(span.TotalMinutes);
            double intseconds = Math.Floor(span.TotalSeconds);
            if (intdays > 0) return intdays + "天";
            else if (inthours > 0) return inthours + "小时";
            else if (intminutes > 0) return intminutes + "分钟";
            else if (intseconds > 0) return intseconds + "秒";
            else return String.Empty;
        }
        /// <summary>
        /// 专制指定日期时间为描述文本
        /// </summary>
        /// <param name="datetime">DateTime 时间</param>
        /// <param name="current">DateTime 时间</param>
        /// <returns>string 描述文本</returns>
        public static string ToTimeTip(DateTime datetime, DateTime current)
        {
            TimeSpan span = datetime - current;
            double intdays = Math.Floor(span.TotalDays);
            double inthours = Math.Floor(span.TotalHours);
            double intminutes = Math.Floor(span.TotalMinutes);
            double intseconds = Math.Floor(span.TotalSeconds);
            if (intdays > 0) return intdays + "天";
            else if (inthours > 0) return inthours + "小时";
            else if (intminutes > 0) return intminutes + "分钟";
            else if (intseconds > 0) return intseconds + "秒";
            else return String.Empty;
        }
        /// <summary>
        /// 转换指定的字符串为字符串数组
        /// </summary>
        /// <param name="str">string 要转换的字符串</param>
        /// <returns>string[] 返回转换后的字符串数组</returns>
        public static string[] ToStrings(string str)
        {
            string strs = ToString(str);
            strs = strs.Replace("\n\r", ",");
            strs = strs.Replace("\r\n", ",");
            strs = strs.Replace("\n", ",");
            strs = strs.Replace("\r", ",");
            strs = strs.Replace("，", ",");
            strs = strs.Replace(";", ",");
            strs = strs.Replace("；", ",");
            strs = strs.Replace(" ", ",");
            strs = strs.Replace("　", ",");
            string[] result = new string[] { };
            string[] strss = strs.Split(System.Convert.ToChar(","));
            for (int i = 0; i < strss.Length; i++)
            {
                if (!Genre.IsNull(strss[i]))
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = strss[i];
                }
            }
            return result;
        }
    }
}
