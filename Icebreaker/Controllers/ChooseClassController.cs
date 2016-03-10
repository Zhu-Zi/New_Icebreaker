using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Icebreaker.Models;
using Icebreaker;
using System.Data.EntityClient;
using XUScoreHeaper;
using Icebreaker.ViewModels;

namespace Icebreaker.Controllers
{
    /// <summary>
    /// 选课情况查询方法
    /// </summary>
    [NoCache]
    [HandleError]
    public class ChooseClassController : Controller 
    {

        public ActionResult Index()
        {
            //ViewData["name"] = Session["name"] as string;
            return View();
        }

        /// <summary>
        /// 生成验证码
        /// 并创建验证码Session
        /// </summary>
        /// <returns></returns>

        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["VaildateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);

            return File(bytes, @"images/jpeg");
        }



        [NoCache]
        [Authorize]
        [AllowAnonymous]//不添加，查询会报错："Http 401"
        public ActionResult GetChooseClassData(ChooseClassViewData e)
        {
            Log log = new Log();
            Student a = new Student(null, null, null);            
            DataTable dtChoseCourse = new DataTable();//ChoseCourseTableDataTable(选课信息的DataTable)
            Icebreaker.Models.IcebreakerEntities1 db = new IcebreakerEntities1();
            string str = null;
            string SchoolYear = null;
            string SchoolTerm = null;

            log.Time = DateTime.Now;
            log.Message = "开始查询成绩";
            log.User = a.ID;
            log.Pwd = a.PWD;
            log.Server = "";
            log.Type = "Info";
            db.Logs.Add(log);
            db.SaveChanges();

            if (Session["VaildateCode"].ToString() != e.Code)
            {
                return View("Index");//修改
            }
            else
            {
                    str = e.server;
                    a.ID = e.User;
                    a.PWD = e.Pwd;
                    SchoolTerm = e.SchoolTerm;
                    switch (e.SchoolYear)
                    {
                        case "1": SchoolYear = "2015-2016"; break;
                        case "2": SchoolYear = "2014-2015"; break;
                        case "3": SchoolYear = "2013-2014"; break;
                        case "4": SchoolYear = "2012-2013"; break;
                    }

                    //      GetTableDbset gt = new GetTableDbset();
                    //      ViewScoreTable vs = new ViewScoreTable();
                    PsaEncyrpt pas = new PsaEncyrpt(); 

                    GetScore gs = new GetScore();
                    try
                    {
                        //dt = gs.GetScoreTable(gs.GetScoreInfoString(a.ID, a.PWD, str)); // todo:catch异常
                        string ScoreData = gs.GetScoreInfoString(a.ID, a.PWD, str);
                        string ChoseClassData = gs.GetElectiveInfoString(a.ID, a.PWD, str,SchoolYear,SchoolTerm);
                        dtChoseCourse = gs.GetElectiveTable(ChoseClassData);              //选课情况
                    }
                    catch (Exception ex)
                    {
                        Log loog = new Log();
                        Icebreaker.Models.IcebreakerEntities1 db2 = new IcebreakerEntities1();

                        loog.Time = DateTime.Now;
                        loog.Type = "Error";
                        loog.Message = ex.Message + "堆栈内容： \n" + ex.StackTrace;
                        loog.User = a.ID;
                        loog.Pwd = pas.Encrypt(a.PWD);
                        loog.Server = str;
                        db2.Logs.Add(loog);
                        db2.SaveChanges();

                        string s = ex.Message;
                        ViewData["ErrorData"] = s;
                        return View("Error");
                    }
                }

                //DataTable列名更改
                //选课情况
                dtChoseCourse.Columns[0].ColumnName = "CourseNumber";
                dtChoseCourse.Columns[1].ColumnName = "CourseName";
                dtChoseCourse.Columns[2].ColumnName = "Teacher";
                dtChoseCourse.Columns[3].ColumnName = "Credit";
                dtChoseCourse.Columns[4].ColumnName = "WeekPeriod";
                dtChoseCourse.Columns[5].ColumnName = "ClassRoom";
                dtChoseCourse.Columns[6].ColumnName = "SchoolTime";



                Icebreaker.Models.ViewModelList vb5 = new ViewModelList();
                List<ViewModels.ViewChoseCourseTable> listChoseClass = vb5.TableToEntity<ViewModels.ViewChoseCourseTable>(dtChoseCourse);

                PsaEncyrpt psaa = new PsaEncyrpt();

                //List<Student> list = db.Student.ToList();
                ViewData["ChoseClass"] = listChoseClass;

                Log looog = new Log();
                Icebreaker.Models.IcebreakerEntities1 db3 = new IcebreakerEntities1();
                looog.Time = DateTime.Now;
                looog.Type = "Info";
                looog.User = a.ID;
                looog.Pwd = psaa.Encrypt(a.PWD);
                looog.Server = str;
                looog.Message = "查询成绩成功,返回" + listChoseClass.Count + "条数据";
                db3.Logs.Add(looog);
                db3.SaveChanges();




            return View("Score");     
        }

    }//主类尾括号
}