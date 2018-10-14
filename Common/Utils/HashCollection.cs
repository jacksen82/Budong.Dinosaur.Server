using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Budong.Common.Utils
{
    /// <summary>
    /// 自定义 Hash 集合
    /// </summary>
    public class HashCollection : List<Hash>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HashCollection()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="array"></param>
        public HashCollection(object[] array)
        {
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != null && array[i].GetType().Name == "Hash")
                    {
                        this.Add((Hash)array[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 查找指定对象在集合中的索引位置
        /// </summary>
        /// <param name="item">Hash 对象</param>
        /// <param name="key">string 键名</param>
        /// <returns>int 索引位置</returns>
        public int IndexOf(Hash item, string key = null)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((key == null && item == this[i])) return i;
                if ((key != null && item.ToString(key).Equals(this[i].ToString(key), StringComparison.CurrentCultureIgnoreCase))) return i;
            }
            return -1;
        }
    }
}
