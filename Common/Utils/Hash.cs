using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 自定义 Hash 类
    /// </summary>
    public class Hash : SortedList, IDictionary
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Hash()
        {

        }
        /// <summary>
        /// 构造函数 [ 通过 JSON 字符串创建 Hash ]
        /// </summary>
        /// <param name="json">string JSON 字符串</param>
        public Hash(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dictionary = (Dictionary<string, object>)serializer.DeserializeObject(json);
            foreach (string key in dictionary.Keys)
            {
                if (dictionary[key] != null && dictionary[key].GetType().Name == "Dictionary`2")
                {
                    this[key] = this.ToHash((Dictionary<string, object>)dictionary[key]);
                }
                else if (dictionary[key] != null && dictionary[key].GetType().Name == "Object[]")
                {
                    this[key] = this.ToArray((object[])dictionary[key]);
                }
                else
                {
                    this[key] = dictionary[key];
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">int 错误码 0 - 成功</param>
        /// <param name="message">string 结果内容</param>
        /// <param name="data">object 附加参数</param>
        public Hash(int code, string message, object data = null)
        {
            this["code"] = code;
            this["message"] = message;
            this["data"] = data;
        }
        /// <summary>
        /// 判断指定键是否为空
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="rigorous">bool 是否严谨模式</param>
        /// <returns>bool 是否为空</returns>
        public bool IsNull(string key, bool rigorous = false)
        {
            if (rigorous == true)
            {
                return this.ContainsKey(key);
            }
            else
            {
                return Utils.Genre.IsNull(this[key]);
            }
        }
        /// <summary>
        /// 判断指定键是否为指定类型
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="type">string 类型</param>
        /// <returns>bool 是否指定类型</returns>
        public bool IsType(string key, string type)
        {
            return Utils.Genre.IsType(this[key], type);
        }
        /// <summary>
        /// 获取指定键的整形值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="def">int 默认值</param>
        /// <returns>int 整形值</returns>
        public int ToInt(string key, int def = 0)
        {
            return Utils.Parse.ToInt(this[key], def);
        }
        /// <summary>
        /// 获取指定键的长整型值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="def">long 默认值</param>
        /// <returns>long 长整型值</returns>
        public long ToLong(string key, long def = 0)
        {
            return Utils.Parse.ToLong(this[key], def);
        }
        /// <summary>
        /// 获取指定键的浮点值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="def">float 默认值</param>
        /// <returns>float 浮点值</returns>
        public float ToFloat(string key, float def = 0)
        {
            return Utils.Parse.ToFloat(this[key], 0, def);
        }
        /// <summary>
        /// 获取指定键的浮点值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="digits">int 精度</param>
        /// <param name="def">float 默认值</param>
        /// <returns>float 浮点值</returns>
        public float ToFloat(string key, int digits, float def = 0)
        {
            return Utils.Parse.ToFloat(this[key], digits, def);
        }
        /// <summary>
        /// 获取指定键的双精度值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <returns>double 双精度值</returns>
        public double ToDouble(string key)
        {
            return Utils.Parse.ToDouble(this[key], 0, 0);
        }
        /// <summary>
        /// 获取指定键的双精度值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="digits">int 精度</param>
        /// <param name="def">double 默认值</param>
        /// <returns>double 双精度值</returns>
        public double ToDouble(string key, int digits, double def = 0)
        {
            return Utils.Parse.ToDouble(this[key], digits, def);
        }
        /// <summary>
        /// 获取指定键的字符串值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="def">string 默认值</param>
        /// <returns>string 字符串值</returns>
        public string ToString(string key, string def = "")
        {
            return Utils.Parse.ToString(this[key], def);
        }
        /// <summary>
        /// 获取指定键的日期时间值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <returns>DateTime 日期时间值</returns>
        public DateTime ToDateTime(string key)
        {
            return Utils.Parse.ToDateTime(this[key], DateTime.Now);
        }
        /// <summary>
        /// 获取指定键的日期时间值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <param name="def">DateTime 默认值</param>
        /// <returns>DateTime 日期时间值</returns>
        public DateTime ToDateTime(string key, DateTime def)
        {
            return Utils.Parse.ToDateTime(this[key], def);
        }
        /// <summary>
        /// 获取指定键的哈希值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <returns>Hash 哈希值</returns>
        public Hash ToHash(string key)
        {
            if (this[key] != null && this[key].GetType().Name == "Hash")
            {
                return (Hash)this[key];
            }
            return new Hash();
        }
        /// <summary>
        /// 获取指定键的哈希集合值
        /// </summary>
        /// <param name="key">string 键名</param>
        /// <returns>HashCollection 哈希集合值</returns>
        public HashCollection ToHashCollection(string key)
        {
            if (this[key] != null)
            {
                if (this[key].GetType().Name == "HashCollection")
                {
                    return (HashCollection)this[key];
                }
                if (this[key].GetType().Name == "Object[]")
                {
                    return new HashCollection((object[])this[key]);
                }
            }
            return new HashCollection();
        }
        /// <summary>
        /// 转换为 JSON 字符串
        /// </summary>
        /// <returns>string JSON 字符串</returns>
        public string ToJSON()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }
        /// <summary>
        /// 转换为 XML 字符串
        /// </summary>
        /// <returns>string XML 字符串</returns>
        public string ToXML()
        {
            StringBuilder xmlString = new StringBuilder();
            xmlString.Append("<xml>");
            foreach (string key in this.Keys)
            {
                xmlString.Append("<" + key + ">" + this[key] + "</" + key + ">");
            }
            xmlString.Append("</xml>");
            return xmlString.ToString();
        }
        /// <summary>
        /// 返回序列化字符串
        /// </summary>
        /// <returns>string 序列化字符串</returns>
        public string ToSerialize()
        {
            List<string> items = new List<string>();
            for (int i = 0; i < this.Keys.Count; i++)
            {
                items.Add(this.GetKey(i) + "=" + this[this.GetKey(i)]);
            }
            return String.Join("&", items.ToArray());
        }
        /// <summary>
        /// 从 XML 文件中获取 Hash 
        /// </summary>
        /// <param name="xml">string xml 内容</param>
        /// <returns>Hash 详细信息</returns>
        public static Hash FromXML(string xml)
        {
            Utils.Hash hashResult = new Utils.Hash();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlElement root = doc.DocumentElement;
            foreach (XmlElement node in root.ChildNodes)
            {
                hashResult.Add(node.Name, node.InnerText);
            }
            return hashResult;
        }

        #region 私有方法
        /// <summary>
        /// 获取对象数组中的词典对象
        /// </summary>
        /// <param name="array">object[] 数组</param>
        private object[] ToArray(object[] array)
        {
            if (array != null)
            {
                object[] objArray = new object[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null && array[i].GetType().Name == "Dictionary`2")
                    {
                        objArray[i] = this.ToHash((Dictionary<string, object>)array[i]);
                    }
                    else if (array[i] != null && array[i].GetType().Name == "Object[]")
                    {
                        objArray[i] = this.ToArray((object[])array[i]);
                    }
                    else
                    {
                        objArray[i] = array[i];
                    }
                }
                return objArray;
            }
            return new object[0];
        }
        /// <summary>
        /// 转换词典对象为哈希对象
        /// </summary>
        /// <param name="dictionary">Dictionary 词典</param>
        /// <returns>Hash 哈希</returns>
        private Hash ToHash(Dictionary<string, object> dictionary)
        {
            Hash result = new Hash();
            foreach (string item in dictionary.Keys)
            {
                if (dictionary[item] != null && dictionary[item].GetType().Name == "Dictionary`2")
                {
                    result[item] = this.ToHash((Dictionary<string, object>)dictionary[item]);
                }
                else if (dictionary[item] != null && dictionary[item].GetType().Name == "Object[]")
                {
                    result[item] = this.ToArray((object[])dictionary[item]);
                }
                else
                {
                    result[item] = dictionary[item];
                }
            }
            return result;
        }
        #endregion
    }
}
