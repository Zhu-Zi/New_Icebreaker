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
    [NoCache]
    [HandleError]
    public class IndexController : Controller
    {

        public ActionResult Index()
        {
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



        /// <summary>
        /// 获得V层数据，并更具Check值执行下步程序
        /// </summary>
        /// <param name="e"></param>
        /// <param name="a"></param>

        [NoCache]
        [Authorize]
        [AllowAnonymous]//不添加，查询会报错："Http 401"
        public ActionResult GetData(ViewData e)
        {
            Log log = new Log();
            Student a = new Student(null, null, null);
            DataTable dt = new DataTable();//scoreDataTable(成绩的DataTable)
            DataTable dtCreditA = new DataTable();//CreditATableDataTable(学分A的DataTable)
            DataTable dtCreditB = new DataTable();//CreditBTableDataTable(学分B的DataTable)
            DataTable dtNotPassedCourse = new DataTable();//NotPassedCourseTableDataTable(未通过课程的DataTable)
            string jidian = null;//接收绩点返回值的变量
            Icebreaker.Models.IcebreakerEntities1 db = new IcebreakerEntities1();
            string str = null;
            //Session["name"] = "Session of Secound ";//创建Session

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
                return View("Index");
            }
            else
            {
                if (e.Check == "remember-me")//数据库备份账号密码，并输出该账号密码的成绩
                {
                    str = e.server;
                    string score = null;
                    GetScore Gs = new GetScore();
                    Icebreaker.Models.Student stu = new Student();
                    Icebreaker.Models.IcebreakerEntities1 d_b = new IcebreakerEntities1();
                    PsaEncyrpt En = new PsaEncyrpt();

                    stu.ID = e.User;
                    stu.PWD = En.Encrypt(e.Pwd);//加密后存入数据库
                    stu.LASTMODIFIED = DateTime.Now;
                    d_b.Students.Add(stu);
                    d_b.SaveChanges();


                    try
                    {
                        Icebreaker.Models.IcebreakerEntities1 dAb = new IcebreakerEntities1();
                        // string EnPwd = dAb.Students.Where(p => p.ID == stu.ID).ToString();

                        List<Icebreaker.Models.Student> list1 = dAb.Students.Where(p => p.ID == stu.ID && p.C_ID > 10).ToList();
                        // string EnPwd = dAb.Students.Where(p => p.ID == stu.ID).ToString();
                        string EnPwd = null;
                        foreach (var aaa in list1)
                        {
                            EnPwd = En.Decrypt(aaa.PWD);//解密输出
                        }

                        string Pwd = EnPwd;

                        score = Gs.GetScoreInfoString(stu.ID, Pwd, str);
                        dt = Gs.GetScoreTable(score);
                        stu.SCORE = score;
                        dAb.Students.Add(stu);
                        dAb.SaveChanges();

                    }
                    catch (Exception Ex)
                    {
                        Log OnLog = new Log();
                        Icebreaker.Models.IcebreakerEntities1 OnDB = new IcebreakerEntities1();

                        OnLog.Time = DateTime.Now;
                        OnLog.Type = "Error";
                        OnLog.Message = Ex.Message + " 堆栈内容：\n" + Ex.StackTrace;
                        OnLog.User = stu.ID;
                        OnLog.Pwd = stu.PWD;
                        OnLog.Server = str;
                        OnDB.Logs.Add(OnLog);
                        OnDB.SaveChanges();

                        string s = Ex.Message;
                        ViewData["ErrorData"] = s;

                    }
                    return View("Error");
                }
                else//请求教务处服务器获得成绩数据并输出
                {
                    str = e.server;
                    a.ID = e.User;
                    a.PWD = e.Pwd;

                    //      GetTableDbset gt = new GetTableDbset();
                    //      ViewScoreTable vs = new ViewScoreTable();
                    PsaEncyrpt pas = new PsaEncyrpt();

                    GetScore gs = new GetScore();
                    try
                    {
                        //dt = gs.GetScoreTable(gs.GetScoreInfoString(a.ID, a.PWD, str)); // todo:catch异常
                        string ScoreData = gs.GetScoreInfoString(a.ID, a.PWD, str);
                        dt = gs.GetScoreTable(ScoreData);                                 //各科成绩                        
                        dtCreditA = gs.GetCharacterTable(ScoreData);                      //学分A：课程性质不同
                        dtCreditB = gs.GetBelongTable(ScoreData);                         //学分B：课程归属不同
                        dtNotPassedCourse = gs.GetFailTable(ScoreData);                   //未通过的课程
                        jidian = gs.GetCensusInfo(ScoreData);                             //绩点

                        try
                        {
                            ScoreView GetText = new ScoreView();                          //创建成绩视图对象
                            Icebreaker.Models.IcebreakerEntities1 dbScoreView = new IcebreakerEntities1();
                            Random ran = new Random();                                    //生成随机数
                            long Singal = ran.Next(10000000, 99999999);
                            PsaEncyrpt EnPas = new PsaEncyrpt();
                            //ViewBag.Singal = Singal.ToString();

                            GetText.Time = DateTime.Now;                                  //数据库写入数据
                            GetText.User = a.ID;
                            //GetText.Text = EnPas.Encrypt(ScoreData);
                            GetText.Text = ScoreData;
                            GetText.Singal = Singal;

                            Session["StudentID"] = a.ID;                                  //创建学号Session
                            Session["Singal"] = Singal;                                   //创建标记Session

                            dbScoreView.ScoreViews.Add(GetText);
                            dbScoreView.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
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
                //各科成绩
                dt.Columns[0].ColumnName = "Year";
                dt.Columns[1].ColumnName = "Semester";
                dt.Columns[2].ColumnName = "Course";
                dt.Columns[3].ColumnName = "Obligatory";
                dt.Columns[4].ColumnName = "ObligatoryClass";
                dt.Columns[5].ColumnName = "Credit";
                dt.Columns[6].ColumnName = "GreadPoint";
                dt.Columns[7].ColumnName = "Score";
                dt.Columns[8].ColumnName = "LastScore";
                dt.Columns[9].ColumnName = "CollageName";


                //学分A
                dtCreditA.Columns[0].ColumnName = "CourseNature";
                dtCreditA.Columns[1].ColumnName = "CreditRequirements";
                dtCreditA.Columns[2].ColumnName = "GotCredit";
                dtCreditA.Columns[3].ColumnName = "NotPassdeCredit";
                dtCreditA.Columns[4].ColumnName = "NeedCredit";

                //学分B
                dtCreditB.Columns[0].ColumnName = "CourseOwnership";
                dtCreditB.Columns[1].ColumnName = "CreditRequirements";
                dtCreditB.Columns[2].ColumnName = "GotCredit";
                dtCreditB.Columns[3].ColumnName = "NotPassdeCredit";
                dtCreditB.Columns[4].ColumnName = "NeedCredit";

                //未通过的课程
                dtNotPassedCourse.Columns[0].ColumnName = "CourseNumber";
                dtNotPassedCourse.Columns[1].ColumnName = "Course";
                dtNotPassedCourse.Columns[2].ColumnName = "Credit";
                dtNotPassedCourse.Columns[3].ColumnName = "CourseNature";
                dtNotPassedCourse.Columns[4].ColumnName = "HighestGrade";


                Icebreaker.Models.ViewModelList vd = new ViewModelList();
                List<ViewModels.ViewScoreTable> list = vd.TableToEntity<ViewModels.ViewScoreTable>(dt);

                Icebreaker.Models.ViewModelList vb2 = new ViewModelList();
                List<ViewModels.ViewCreditATable> listCreditA = vb2.TableToEntity<ViewModels.ViewCreditATable>(dtCreditA);

                Icebreaker.Models.ViewModelList vb3 = new ViewModelList();
                List<ViewModels.ViewCreditBTable> listCreditB = vb3.TableToEntity<ViewModels.ViewCreditBTable>(dtCreditB);

                Icebreaker.Models.ViewModelList vb4 = new ViewModelList();
                List<ViewModels.ViewNotPassedCourseTable> listNoPassClass = vb4.TableToEntity<ViewModels.ViewNotPassedCourseTable>(dtNotPassedCourse);


                PsaEncyrpt psaa = new PsaEncyrpt();

                //           List<Student> list = db.Student.ToList();
                ViewData["TableData"] = list;
                ViewData["CreditA"] = listCreditA;
                ViewData["CreditB"] = listCreditB;
                ViewData["NoPassClass"] = listNoPassClass;
                ViewData["JiDian"] = jidian;

                Log looog = new Log();
                Icebreaker.Models.IcebreakerEntities1 db3 = new IcebreakerEntities1();
                looog.Time = DateTime.Now;
                looog.Type = "Info";
                looog.User = a.ID;
                looog.Pwd = psaa.Encrypt(a.PWD);
                looog.Server = str;
                looog.Message = "查询成绩成功,返回" + list.Count + "条数据";
                db3.Logs.Add(looog);
                db3.SaveChanges();


                //var aaaaa = from c in db3.Logs
                //          where c.ID == 1
                //          select c;              
                return View("Score");

            }
        }

        public string Get(ViewData e)
        {
            return "Hello";
        }

    }//主类尾括号

    /// <summary>
    /// 添加此类后可以使用System.web.mvc中的“[Nocache]”属性
    /// 由此可以清除客户端浏览器上的缓存
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}