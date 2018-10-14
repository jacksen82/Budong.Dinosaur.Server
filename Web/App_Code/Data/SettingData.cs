using System;
using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 小程序应用操作类
/// </summary>
public class SettingData
{
    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <returns>Hash 配置信息</returns>
    public static Hash Detail()
    {
        string sql = "SELECT * FROM tz_setting";
        using (MySqlADO ado = new MySqlADO())
        {
            Hash result = new Hash();
            HashCollection items = ado.GetHashCollection(sql).ToHashCollection("data");
            foreach (Hash item in items)
            {
                result[item.ToString("key")] = item.ToString("value");
            }
            return result;
        }
    }
    /// <summary>
    /// 设置配置项目
    /// </summary>
    /// <param name="key">string 键名</param>
    /// <param name="value">string 键值</param>
    /// <returns>int 受影响的行数</returns>
    public static int SetItem(string key, string value)
    {
        string sql = "UPDATE tz_setting SET `value`=@1 WHERE `key`=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, key, value);
        }
    }
}