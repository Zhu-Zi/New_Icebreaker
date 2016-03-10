using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using Icebreaker.ViewModels;


namespace Icebreaker.Models
{
    public class ViewModelList
    {
        /// <summary>
        /// DataTable 转 list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public  List<T> TableToEntity<T>(DataTable dt) where T : new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            string tempName = string.Empty; 
            foreach(DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();

                foreach (PropertyInfo p in pArray)
                {
                    tempName = p.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        if (!p.CanWrite)
                        {
                            continue;
                        }
                        object value = row[tempName];
                        if (value != DBNull.Value)
                        {
                            p.SetValue(entity, value, null);
                        }
                              
                    }
                }
                list.Add(entity);
            }
            return list;
        }
    }
}