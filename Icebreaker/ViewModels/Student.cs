using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Icebreaker.Models
{
    public partial class Student
    {

        string UrlString = null;//url字符串
        string TableString = null;//html字符串
        public Student()
        { 
        
        }
        public Student(string id, string pwd, string urlString)
        {//构造函数
            this.ID = id;
            this.PWD = pwd;
            this.UrlString = urlString;
        }
        public string GetScoreHtml()
        {
            this.TableString = new XUScoreHeaper.GetScore().GetScoreInfoString(this.ID, this.PWD, this.UrlString);//初始化赋值
            return TableString;
        }
        public DataTable GetScoreTable()
        {
            if (TableString == null)
            {
                throw new Exception("请先使用GetScoreHtml获取成绩");
            }
            else
            {
                try
                {
                    return new XUScoreHeaper.GetScore().GetScoreTable(this.TableString);
                }
                catch (Exception e)
                {
                    throw new Exception("将成绩解析为DataTable时出错！");

                }
            }
        } 

    }//主类尾括号
}