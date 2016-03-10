using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icebreaker.ViewModels
{
    public class ChooseClassViewData
    {
        public string User { get; set; }
        public string Pwd { get; set; }
        public string server { get; set; }
        public string Code { get; set; }
        public string SchoolYear { get; set; }
        public string SchoolTerm { get; set; }
    }
}