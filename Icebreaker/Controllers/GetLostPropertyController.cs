using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.EntityClient;
using Icebreaker;
using Icebreaker.ViewModels;
using Icebreaker.Models;
using XUScoreHeaper;


namespace Icebreaker.Controllers
{
    public class GetLostPropertyController : Controller
    {
        public bool PhotoUrlSignal = false;//图片上传信号
        public string PhotoUrl = null;//图片链接
        //GET:GetLostProperty
        public ActionResult Index()
        {
            return View("Index");
        }


        /// <summary>
        /// 数据库读取数据
        /// 返回给前端显示
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayLostPropertyData()
        {
            Icebreaker.Models.IcebreakerEntities1 db_LostData = new IcebreakerEntities1();
            List<Models.LostProperty> LostPropertyList = db_LostData.LostProperties.Where(p => p.Condition == 0).ToList();

            ViewData["LostPropertyData"] = LostPropertyList;
            return View("DisplayLostPropertyData");
        }


        /// <summary>
        /// 发布招领失物信息
        /// 描述失物丢失地点，时间。上传失物图片。
        /// 填写个人联系方式和常用邮箱。
        /// 日志通过数据库的触发器写入PublishLog
        /// </summary>
        /// <returns></returns>
        public ActionResult PublishLostProperty()
        {
            return View("PublishLostProperty");
        }
        [HttpPost]
        public ActionResult PublishLostPropertyData(PublishLostPropertyViewData e, HttpPostedFileBase upImg)
        {
            LostProperty LostData = new LostProperty();//实例化失物信息类
            Icebreaker.Models.Publisher PublisherData = new Models.Publisher();//实例化发布者信息
            GetScore GS = new GetScore();//实例化验证对象
            JsonResult JsonStr = new JsonResult();//图片上传返回字符串


            bool StudentCondition = new bool();//是否为学生
            StudentCondition = GS.CheckLogin(e.PublisherStuNum, e.Passward, "172.17.1.41");//使用学号密码，通过教务系统实现对学生的验证

            //根据返回结果判断是否为学生
            if (StudentCondition == true)
            {
                Icebreaker.Models.IcebreakerEntities1 db_Publisher = new IcebreakerEntities1();
                Icebreaker.Models.IcebreakerEntities1 db_LostData = new IcebreakerEntities1();


                ///接收数据/////////////////////////////////////////////////////////////////////

                JsonStr = Upload(upImg);//调用图片上传方法

                PublisherData.PublisherStuNum = e.PublisherStuNum;//学号
                PublisherData.PublisherPhone = e.PublisherPhone;//联系方式
                PublisherData.PublisherE_mail = e.PublisherE_mail;//邮件地址

                LostData.Name = e.Name;//失物名称
                LostData.PublishDate = DateTime.Now;
                //LostData.PhotoUrl = null;//图片链接
                if (PhotoUrlSignal == true)
                {
                    LostData.PhotoUrl = PhotoUrl;
                }
                else
                {
                    LostData.PhotoUrl = null;//图片链接
                }

                LostData.PublisherNum = e.PublisherStuNum;//发布者编号(学号)
                LostData.Describtion = e.Describtion;//失物信息描述
                LostData.Condition = 0;//失物状态

                //数据写入数据库

                db_LostData.LostProperties.Add(LostData);//失物信息写入数据库
                db_LostData.SaveChanges();

                List<Models.Publisher> Publisher = db_Publisher.Publishers.Where(p => p.PublisherStuNum == PublisherData.PublisherStuNum).ToList();//从数据库取出该编号的记录
                if (Publisher.Count == 0)
                {
                    db_Publisher.Publishers.Add(PublisherData);
                    db_Publisher.SaveChanges();
                }
                return View("Index");
            }
            else
            {
                return View("Error");
            }
        }


        /// <summary>
        /// 实现上传图片
        /// </summary>
        //[HttpPost]
        public JsonResult Upload(HttpPostedFileBase upImg)
        {
            if (upImg != null)//判断是否有图片
            {
                string fileName = System.IO.Path.GetFileName(upImg.FileName);//文件名
                string filePhysicalPath = Server.MapPath("~/images/" + fileName);//文件物理路径               
               
                string pic = "", error = "";
                try
                {
                    upImg.SaveAs(filePhysicalPath);
                    pic = "/images/" + fileName;
                    PhotoUrlSignal = true;
                    filePhysicalPath = filePhysicalPath.Replace("\\", "/");
                    filePhysicalPath = filePhysicalPath.Substring(28);
                    PhotoUrl = filePhysicalPath;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                return Json(new
                {
                    pic = pic,
                    error = error
                });
            }
            else
            {
                return Json(new
                {
                    pic = "",
                    error = ""
                });
            }

        }


        //////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 认领失物
        /// 通过学号密码，或者QQ确认认领人信息
        /// 既若通过学号密码验证则采用QQ验证，反之则采用QQ验证
        /// 验证通过后，返回失物招领人的联系方式
        /// 失物进入待审核状态，发布人审核或者邮件审核
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLostProperty()
        {
            return View("GetLostProperty");
        }

        public ActionResult ReceiverData(ReceiverViewData e,DisplayLostPropertyDataViewData a)
        {
            Icebreaker.Models.Receiver ReceiverData = new Models.Receiver();//实例化领取人信息类
            //Icebreaker.ViewModels.Receiver ReceiverPWD = new ViewModels.Receiver();//实例化领取人密码类
            ReceiveLog Log = new ReceiveLog();//实例化领取人日志类
            //GetScore GS = new GetScore();//实例化验证类

            //bool StudentCondition = new bool();
            //StudentCondition = GS.CheckLogin(e.ReceiverStuNum, e.Passward, "172.17.1.41");

            //if (StudentCondition == true)
            //{
            //    Icebreaker.Models.IcebreakerEntities1 db_Receiver = new IcebreakerEntities1();
            //    Icebreaker.Models.IcebreakerEntities1 db_Log = new IcebreakerEntities1();

            //    ReceiverData.ReceiverStuNum = e.ReceiverStuNum;
            //    ReceiverData.ReceiverPhone = e.ReceiverPhone;

            //    Log.LostPropertyNum = 0;
            //    Log.ReceiverNum = ReceiverData.ReceiverStuNum;
            //    Log.ReceiverDate = DateTime.Now;
            //    Log.Condition = 0;//领取物品状态改变为0，即审核状态

            //    //db_Receiver.Receivers.Add(ReceiverData);
            //    //db_Receiver.SaveChanges();

            //    //db_Log.ReceiveLogs.Add(Log);
            //    //db_Log.SaveChanges();

            //    return View();
            //}
            //else
            //{
            //    return View("Error");
            //}
            Icebreaker.Models.IcebreakerEntities1 db_Receiver = new IcebreakerEntities1();
            Icebreaker.Models.IcebreakerEntities1 db_Log = new IcebreakerEntities1();

            ReceiverData.ReceiverStuNum = e.ReceiverStuNum;//学号
            ReceiverData.ReceiverPhone = e.ReceiverPhone;//联系方式

            Log.LostPropertyNum = Convert.ToInt32(a.DisplayNumber);//货物编号
            Log.ReceiverNum = ReceiverData.ReceiverStuNum;//认领人编号（学号）
            Log.ReceiverDate = DateTime.Now;
            Log.Condition = 0;//领取物品状态改变为0，即审核状态

            List<Models.Receiver> Receiver = db_Receiver.Receivers.Where(p=>p.ReceiverStuNum == ReceiverData.ReceiverStuNum).ToList();//从数据库取出该编号的记录
            if (Receiver.Count == 0)
            {
                db_Receiver.Receivers.Add(ReceiverData);
                db_Receiver.SaveChanges();
            }


            db_Log.ReceiveLogs.Add(Log);
            db_Log.SaveChanges();

            return View("Index");
        }

    }//主类尾括号
}