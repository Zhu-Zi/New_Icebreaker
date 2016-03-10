using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icebreaker.ViewModels
{
    public class ViewNotPassedCourseTable
    {
        public string CourseNumber { get; set; }//课程代码
        public string Course { get; set; }//课程名称
        public string Credit { get; set; }//学分
        public string CourseNature { get; set; }//课程性质
        public string HighestGrade { get; set; }//最高成绩
    }
}