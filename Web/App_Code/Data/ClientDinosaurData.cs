using System;
using Budong.Common.Data;
using Budong.Common.Utils;

/// <summary>
/// 客户端题目操作类
/// </summary>
public class ClientDinosaurData
{
    /// <summary>
    /// 获取客户端进度信息
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>Hash 进度信息</returns>
    public static Hash GetByClientId(int clientId)
    {
        string sql = "SELECT clientId,score, " +
            "   (SELECT COUNT(*) FROM tc_client_dinosaur WHERE clientId=@0 AND result=1) AS doneCount," +
            "   (SELECT COUNT(*) FROM td_dinosaur) AS allCount " +
            "FROM tc_client WHERE clientId=@0 ";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.GetHash(sql, clientId);
        }
    }
    /// <summary>
    /// 获取剩余题目
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>Hash 题目集合</returns>
    public static Hash Assign(int clientId)
    {
        string sql = "SELECT td.*,tcd.result " +
            "FROM td_dinosaur td LEFT JOIN tc_client_dinosaur tcd ON td.dinosaurId=tcd.dinosaurId AND clientId=@0 " +
            "WHERE IFNULL(tcd.result, 0) in (0,2) " +
            "ORDER BY IFNULL(tcd.result, 0) DESC, td.index ASC ";
        using (MySqlADO ado = new MySqlADO())
        {
            HashCollection items = ado.GetHashCollection(sql, clientId).ToHashCollection("data");
            if (items.Count > 0)
            {
                if (items[0].ToInt("result") == 2)
                {
                    return items[0];
                }
                Random random = new Random();
                return items[random.Next(0, items.Count)];
            }
            return new Hash();
        }
    }
    /// <summary>
    /// 答题结果
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <param name="dinosaurId">int 题目编号</param>
    /// <param name="result">int 答题结果</param>
    /// <returns>int 受影响的行数</returns>
    public static int Answer(int clientId, int dinosaurId, int result)
    {
        string sql = "INSERT INTO tc_client_dinosaur (clientId,dinosaurId,result) " +
            "VALUES(@0,@1,@2) " +
            "ON DUPLICATE KEY UPDATE clientId=VALUES(clientId),dinosaurId=VALUES(dinosaurId),result=VALUES(result),createTime=Now()";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId, dinosaurId, result);
        }
    }
    /// <summary>
    /// 重新开始
    /// </summary>
    /// <param name="clientId">int 客户端编号</param>
    /// <returns>int 受影响的行数</returns>
    public static int Restart(int clientId)
    {
        string sql = "DELETE FROM tc_client_dinosaur WHERE clientId=@0";
        using (MySqlADO ado = new MySqlADO())
        {
            return ado.NonQuery(sql, clientId);
        }
    }
}