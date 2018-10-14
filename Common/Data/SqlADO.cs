using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;

namespace Budong.Common.Data
{
    /// <summary>
    /// Sql 数据库操作类
    /// </summary>
    public class SqlADO : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlADO()
        {
            this._ConnectionString = ConfigurationManager.ConnectionStrings["ConnectString"].ConnectionString;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlADO(string connectionString)
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
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                using (SqlCommand comd = new SqlCommand(sql, conn))
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
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                using (SqlCommand comd = new SqlCommand(sql, conn))
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
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                using (SqlCommand comd = new SqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (SqlDataReader reader = comd.ExecuteReader())
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
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                using (SqlCommand comd = new SqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (SqlDataReader reader = comd.ExecuteReader())
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
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                using (SqlCommand comd = new SqlCommand(sql, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (SqlDataReader reader = comd.ExecuteReader())
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
                            result["Data"] = data;
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
        /// <param name="pageID">int 页码</param>
        /// <param name="pageSize">int 页尺寸</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>Utils.HashCollection 记录集合</returns>
        public Utils.Hash GetHashCollection(string sql, int pageID, int pageSize, params object[] values)
        {
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                Match match = Regex.Match(sql, "Order By [^()]*$", RegexOptions.IgnoreCase);
                int rowStart = ((pageID - 1) * pageSize) + 1;
                int rowEnd = rowStart + pageSize - 1;
                int recordCount = 0;
                string sqlOrder = "Order By 1 Asc";
                string sqlCount = "Select Count(*) As Count From ({SQL}) T";
                string sqlSelect = "Select * From (Select *,ROW_NUMBER() OVER ({ORDER}) AS [RowIndex] From ({SQL}) T) TT Where [RowIndex] Between " + rowStart + " And " + rowEnd + " Order By [RowIndex] Asc";
                if (match.Success)
                {
                    sql = sql.Replace(match.Value, "");
                    sqlOrder = match.Value;
                }
                sqlCount = sqlCount.Replace("{SQL}", sql);
                sqlCount = sqlCount.Replace("{ORDER}", sqlOrder);
                sqlSelect = sqlSelect.Replace("{SQL}", sql);
                sqlSelect = sqlSelect.Replace("{ORDER}", sqlOrder);
                using (SqlCommand comd = new SqlCommand(sqlSelect, conn))
                {
                    this.AddParameters(comd, values);
                    conn.Open();
                    try
                    {
                        using (SqlCommand _comd = new SqlCommand(sqlCount, conn))
                        {
                            recordCount = Utils.Parse.ToInt(_comd.ExecuteScalar());
                        }
                        using (SqlDataReader reader = comd.ExecuteReader())
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
                            result["Data"] = data;
                            result["Count"] = recordCount;
                            result["PageCount"] = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(recordCount) / Convert.ToDouble(pageSize)));
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
        /// 执行 T-SQL 语句，并返回数据库记录表
        /// </summary>
        /// <param name="sql">string T-SQL 语句</param>
        /// <param name="pageID">int 页码</param>
        /// <param name="pageSize">int 页尺寸</param>
        /// <param name="values">params object[] 参数集合 </param>
        /// <returns>DataSet 数据集</returns>
        public DataSet GetDataSet(string sql, int pageID, int pageSize, params object[] values)
        {
            using (SqlConnection conn = new SqlConnection(this._ConnectionString))
            {
                Match match = Regex.Match(sql, "Order By [^()]*$", RegexOptions.IgnoreCase);
                int rowStart = ((pageID - 1) * pageSize) + 1;
                int rowEnd = rowStart + pageSize - 1;
                int recordCount = 0;
                int pageCount = 0;
                string sqlFull = "";
                string sqlOrder = "Order By 1 Asc";
                string sqlCount = "Select Count(*) As Count From ({SQL}) T";
                string sqlSelect = "Select * From (Select *,ROW_NUMBER() OVER ({ORDER}) AS [RowIndex] From ({SQL}) T) TT Where [RowIndex] Between " + rowStart + " And " + rowEnd + " Order By [RowIndex] Asc";
                if (match.Success)
                {
                    sql = sql.Replace(match.Value, "");
                    sqlOrder = match.Value;
                }
                sqlFull = (sqlCount + "\n" + sqlSelect);
                sqlFull = sqlFull.Replace("{SQL}", sql);
                sqlFull = sqlFull.Replace("{ORDER}", sqlOrder);
                using (SqlCommand comd = new SqlCommand(sqlFull, conn))
                {
                    DataSet dataResult = new DataSet();

                    this.AddParameters(comd, values);
                    conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(comd);
                    adapter.Fill(dataResult);
                    conn.Close();
                    if (dataResult.Tables.Count > 0)
                    {
                        recordCount = Utils.Parse.ToInt(dataResult.Tables[0].Rows[0]["Count"]);
                        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(recordCount) / Convert.ToDouble(pageSize)));
                        dataResult.Tables[0].TableName = "DataCount";
                    }
                    if (dataResult.Tables.Count > 1)
                    {
                        dataResult.Tables[1].TableName = "DataList";
                    }

                    this.AddPages(dataResult, pageID, pageCount);

                    return dataResult;
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
        /// <param name="comd">SqlCommand Sql 对象</param>
        private void AddParameters(SqlCommand comd, params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    comd.Parameters.Add(new SqlParameter("@" + i, System.DBNull.Value));
                }
                else
                {
                    switch (values[i].GetType().Name)
                    {
                        case "Int16": comd.Parameters.Add("@" + i, SqlDbType.SmallInt).Value = values[i]; break;
                        case "Int32": comd.Parameters.Add("@" + i, SqlDbType.Int).Value = values[i]; break;
                        case "Int64": comd.Parameters.Add("@" + i, SqlDbType.BigInt).Value = values[i]; break;
                        case "Boolean": comd.Parameters.Add("@" + i, SqlDbType.Bit).Value = values[i]; break;
                        case "String": comd.Parameters.Add("@" + i, SqlDbType.VarChar).Value = this.Clean((string)values[i]); break;
                        case "Guid": comd.Parameters.Add("@" + i, SqlDbType.UniqueIdentifier).Value = values[i]; break;
                        case "Byte": comd.Parameters.Add("@" + i, SqlDbType.TinyInt).Value = values[i]; break;
                        case "Single": comd.Parameters.Add("@" + i, SqlDbType.Real).Value = values[i]; break;
                        case "Double": comd.Parameters.Add("@" + i, SqlDbType.Float).Value = values[i]; break;
                        case "Decimal": comd.Parameters.Add("@" + i, SqlDbType.Decimal).Value = values[i]; break;
                        case "DateTime": comd.Parameters.Add("@" + i, SqlDbType.DateTime).Value = values[i]; break;
                        default: comd.Parameters.Add(new SqlParameter("@" + i, System.DBNull.Value)); break;
                    }
                }
            }
        }
        /// <summary>
        /// 生成分页表
        /// </summary>
        /// <param name="data">DataSet 数据集</param>
        /// <param name="pageID">int 页码</param>
        /// <param name="pageCount">int 总页数</param>
        /// <returns>DataTable 数据集</returns>
        private void AddPages(DataSet data, int pageID, int pageCount)
        {
            if (HttpContext.Current != null)
            {
                string urlPath = HttpContext.Current.Request.Url.AbsolutePath;
                string urlQuery = urlPath + "?PageID={PageID}";
                foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (!Utils.Genre.IsEquals(key, "PageID"))
                    {
                        urlQuery += (urlQuery.EndsWith("&") ? "" : "&") + key + "=" + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.QueryString[key]);
                    }
                }

                DataTable tablePages = new DataTable();
                tablePages.TableName = "DataPage";
                tablePages.Columns.Add("Url", Type.GetType("System.String"));
                tablePages.Columns.Add("PageID", Type.GetType("System.Int32"));
                tablePages.Columns.Add("PageText", Type.GetType("System.String"));
                tablePages.Columns.Add("PageClass", Type.GetType("System.String"));

                int pageStart = (pageID > 5 ? pageID - 5 : 1);
                int pageEnd = (pageCount > pageStart + 10 ? pageStart + 10 : pageCount + 1);

                if (pageID > 1)
                {
                    DataRow rowPrev = tablePages.NewRow();
                    rowPrev["Url"] = urlQuery.Replace("{PageID}", (pageID - 1).ToString());
                    rowPrev["PageID"] = (pageID - 1);
                    rowPrev["PageText"] = "上一页";
                    rowPrev["PageClass"] = "prev";
                    tablePages.Rows.Add(rowPrev);
                }
                for (int i = pageStart; i < pageEnd; i++)
                {
                    DataRow rowItem = tablePages.NewRow();
                    rowItem["Url"] = urlQuery.Replace("{PageID}", i.ToString());
                    rowItem["PageID"] = i;
                    rowItem["PageText"] = i;
                    rowItem["PageClass"] = (i == pageID ? "item selected" : "item");
                    tablePages.Rows.Add(rowItem);
                }
                if (pageCount > pageID)
                {
                    DataRow rowNext = tablePages.NewRow();
                    rowNext["Url"] = urlQuery.Replace("{PageID}", (pageID + 1).ToString());
                    rowNext["PageID"] = (pageID + 1);
                    rowNext["PageText"] = "下一页";
                    rowNext["PageClass"] = "next";
                    tablePages.Rows.Add(rowNext);
                }
                data.Tables.Add(tablePages);
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
