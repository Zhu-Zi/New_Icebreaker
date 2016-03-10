using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icebreaker.ViewModels
{
    public class PublishLostPropertyViewData
    {
        public string PublisherStuNum { get; set; }//学号(发布者编号)
        public string Passward { get; set; }//密码
        public string PublisherPhone { get; set; }//发布者联系方式
        public string PublisherE_mail { get; set; }//发布者邮件地址
        public string Name { get; set; }//失物名称
        public string Describtion { get; set; }//失物描述
        public string PhotoUrl { get; set; }//图片链接

    }
}