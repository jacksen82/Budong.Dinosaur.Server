using System;
using System.IO;
using System.Web;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath">string 文件路径</param>
        /// <returns>bool 是否存在</returns>
        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
        /// <summary>
        /// 以时间戳生成文件名
        /// </summary>
        /// <param name="extension">string 扩展名(带.)</param>
        /// <returns>string 文件名</returns>
        public static string NewFileName(string extension)
        {
            return DateTime.Now.ToString("yyMMddHHmmssms") + extension;
        }
        /// <summary>
        /// 根据虚拟地址获得绝对路径
        /// </summary>
        /// <param name="fileUrl">string 虚拟地址</param>
        /// <returns>string 绝对路径</returns>
        public static string MapPath(string fileUrl)
        {
            return HttpContext.Current.Server.MapPath(fileUrl);
        }
        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="filePath">string 文件路径</param>
        /// <returns>string 后缀名 ( 不带 . )</returns>
        public static string GetExtension(string filePath)
        {
            if (Genre.IsNull(filePath))
            {
                return String.Empty;
            }
            return Path.GetExtension(filePath);
        }
        /// <summary>
        /// 根据虚拟地址删除文件
        /// </summary>
        /// <param name="fileUrl">string 虚拟地址</param>
        public static void Delete(string fileUrl)
        {
            try
            {
                File.Delete(MapPath(fileUrl));
            }
            catch { }
        }
        /// <summary>
        /// 保存二级制数据至文件
        /// </summary>
        /// <param name="filePath">string 文件路径(绝对路径)</param>
        /// <param name="bytes">byte[] 数据流</param>
        public static void SaveAllBytes(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }
    }
}
