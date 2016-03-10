using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icebreaker.ViewModels
{
    public class ViewChoseCourseTable
    {
        public string CourseNumber { get; set; }//选课课号
        public string CourseName { get; set; }//课程名称
        public string Teacher { get; set; }//教师姓名
        public string Credit { get; set; }//学分
        public string WeekPeriod { get; set; }//周学时
        public string ClassRoom { get; set; }//上课地点
        public string SchoolTime { get; set; }//上课时间
    }
}