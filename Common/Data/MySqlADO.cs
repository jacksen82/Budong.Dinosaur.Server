using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Budong.Common.Data
{
    /// <summary>
    /// MySql 数据库操作类
    /// </summary>
    public class MySqlADO : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MySqlADO()
        {
            this._ConnectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public MySqlADO(string connectionString)
        {
            this._ConnectionString = connectionString;
        }

        /// <summary>
        /// 执行 T-SQL 语句，并返回受影响的行数
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>int 受影响的行数</returns>
        public int NonQuery(string sql, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        return comd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首行首列的值 [ int 值 ]
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>int 首行首列的值</returns>
        public int GetInt(string sql, params object[] values)
        {
            return Utils.Parse.ToInt(this.GetValue(sql, values));
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首行首列的值 [ double 值 ]
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="digits">int 精度</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>int 首行首列的值</returns>
        public double GetDouble(string sql, int digits, params object[] values)
        {
            return Utils.Parse.ToDouble(this.GetValue(sql, values), digits);
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首行首列的值 [ string 值 ]
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>string 首行首列的值</returns>
        public string GetString(string sql, params object[] values)
        {
            return Utils.Parse.ToString(this.GetValue(sql, values));
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首行首列的值
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>object 首行首列的值</returns>
        public object GetValue(string sql, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        return comd.ExecuteScalar();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首列的值数组
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>object[] 首列的值数组</returns>
        public object[] GetValues(string sql, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (MySqlDataReader reader = comd.ExecuteReader())
                        {
                            object[] result = new object[0];
                            while (reader.Read())
                            {
                                Array.Resize<object>(ref result, result.Length + 1);
                                result[result.Length - 1] = reader.GetValue(0);
                            }
                            reader.Close();
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库首行首列的值 [ DateTime 值 ]
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>int 首行首列的值</returns>
        public DateTime GetDateTime(string sql, params object[] values)
        {
            return Utils.Parse.ToDateTime(this.GetValue(sql, values));
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回首行记录
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>Utils.Hash 首行记录集合</returns>
        public Utils.Hash GetHash(string sql, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (MySqlDataReader reader = comd.ExecuteReader())
                        {
                            Utils.Hash result = new Utils.Hash();
                            if (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.GetValue(i) == DBNull.Value && reader.GetFieldType(i).Name == "String")
                                    {
                                        result[reader.GetName(i)] = String.Empty;
                                    }
                                    else
                                    {
                                        result[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                }
                            }
                            reader.Close();
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库记录集合
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>Utils.HashCollection 记录集合</returns>
        public Utils.Hash GetHashCollection(string sql, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (MySqlDataReader reader = comd.ExecuteReader())
                        {
                            Utils.Hash result = new Utils.Hash();
                            Utils.HashCollection data = new Utils.HashCollection();
                            while (reader.Read())
                            {
                                Utils.Hash item = new Utils.Hash();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.GetValue(i) == DBNull.Value && reader.GetFieldType(i).Name == "String")
                                    {
                                        item[reader.GetName(i)] = String.Empty;
                                    }
                                    else
                                    {
                                        item[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                }
                                data.Add(item);
                            }
                            reader.Close();
                            result["data"] = data;
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行 T-SQL 语句，并返回数据库记录集合 [ 含分页 ]
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <param name="pageId">int 页码</param>
        /// <param name="pageSize">int 页尺寸</param>
        /// <returns>Utils.HashCollection 记录集合</returns>
        public Utils.Hash GetHashCollectionByPageId(string sql, int pageId, int pageSize, params object[] values)
        {
            using (MySqlConnection conn = new MySqlConnection(this._ConnectionString))
            {
                using (MySqlCommand comd = new MySqlCommand(sql, conn))
                {
                    double recordCount = 0;
                    double recordStart = ((pageId - 1) * pageSize);
                    double recordEnd = recordStart + pageSize;

                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (MySqlDataReader reader = comd.ExecuteReader())
                        {
                            Utils.Hash result = new Utils.Hash();
                            Utils.HashCollection data = new Utils.HashCollection();
                            while (reader.Read())
                            {
                                if (recordCount >= recordStart && recordCount < recordEnd)
                                {
                                    Utils.Hash item = new Utils.Hash();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (reader.GetValue(i) == DBNull.Value && reader.GetFieldType(i).Name == "String")
                                        {
                                            item[reader.GetName(i)] = String.Empty;
                                        }
                                        else
                                        {
                                            item[reader.GetName(i)] = reader.GetValue(i);
                                        }
                                    }
                                    data.Add(item);
                                }
                                recordCount++;
                            }
                            reader.Close();
                            result["data"] = data;
                            result["pageCount"] = Math.Ceiling(recordCount / pageSize);
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        #region 私有变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _ConnectionString;
        #endregion

        #region 私有方法
        /// <summary>
        /// 过滤 T-Sql 字符串
        /// </summary>
        /// <param name="str">string 字符串参数</param>
        /// <returns>string 过滤结果</returns>
        public string Clean(string str)
        {
            return Regex.Replace((string)str, "exec insert |select |delete |'|update |chr\\(|master\\(|mid\\(|truncate\\(|char\\(|declare\\(| or | and 	|;and | backup | nchar |drop table |delete from |net user | net 	localgroup |xp_cmdshell|<script>|</script>", "", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 过滤 T-SQL 语句中的表不安全语句
        /// </summary>
        /// <param name="values">params object[] 参数集合 </param>
        /// <param name="comd">MySqlCommand Sql 对象</param>
        private void AddParameters(MySqlCommand comd, params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    comd.Parameters.Add(new MySqlParameter("@" + i, System.DBNull.Value));
                }
                else
                {
                    switch (values[i].GetType().Name)
                    {
                        case "Int16": comd.Parameters.Add("@" + i, MySqlDbType.Int16).Value = values[i]; break;
                        case "Int32": comd.Parameters.Add("@" + i, MySqlDbType.Int32).Value = values[i]; break;
                        case "Int64": comd.Parameters.Add("@" + i, MySqlDbType.Int64).Value = values[i]; break;
                        case "Boolean": comd.Parameters.Add("@" + i, MySqlDbType.Bit).Value = values[i]; break;
                        case "String": comd.Parameters.Add("@" + i, MySqlDbType.VarChar).Value = this.Clean((string)values[i]); break;
                        case "Guid": comd.Parameters.Add("@" + i, MySqlDbType.VarChar).Value = values[i]; break;
                        case "Byte": comd.Parameters.Add("@" + i, MySqlDbType.Byte).Value = values[i]; break;
                        case "Single": comd.Parameters.Add("@" + i, MySqlDbType.Int32).Value = values[i]; break;
                        case "Double": comd.Parameters.Add("@" + i, MySqlDbType.Float).Value = values[i]; break;
                        case "Decimal": comd.Parameters.Add("@" + i, MySqlDbType.Decimal).Value = values[i]; break;
                        case "DateTime": comd.Parameters.Add("@" + i, MySqlDbType.DateTime).Value = values[i]; break;
                        default: comd.Parameters.Add(new SqlParameter("@" + i, System.DBNull.Value)); break;
                    }
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {

        }
        #endregion
    }
}
