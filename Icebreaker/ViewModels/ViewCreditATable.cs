using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icebreaker.ViewModels
{
    public class ViewCreditATable
    {
        public string CourseNature { get; set; }//课程性质
        public string CreditRequirements { get; set; }//学分要求
        public string GotCredit { get; set; }//获得学分
        public string NotPassdeCredit { get; set; }//未通过学分
        public string NeedCredit { get; set; }//还需学分
    }
}