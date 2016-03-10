using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icebreaker.ViewModels
{
    public class ReceiverViewData
    {
        public string ReceiverStuNum { get; set; }//学号
        public string ReceiverPhone { get; set; }//联系方式
        public string Passward { get; set; }//密码
    }
}