using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 题目操作类
/// </summary>
public class DinosaurData
{
    /// <summary>
    /// 获取所有题目选项
    /// </summary>
    /// <param name="exceptDinosaurId">int 排除题目编号</param>
    /// <returns>Hash 题目集合</returns>
    public static Hash AllName(int exceptDinosaurId)
    {
        string sql = "SELECT name FROM td_dinosaur WHERE dinosaurId<>@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHashCollection(sql, exceptDinosaurId);
        }
    }
}