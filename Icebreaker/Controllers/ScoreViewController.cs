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
using Icebreaker.Assists;

namespace Icebreaker.Controllers
{
    public class ScoreViewController : Controller
    {
        public ActionResult Index()
        {
            string user = null;
            string singal = null;
            user = Session["StudentID"].ToString();
            singal = Session["Singal"].ToString();

            DataTable dt = new DataTable();
            GetScore gs = new GetScore();
            PsaEncyrpt psa = new PsaEncyrpt();
            long Singal = Convert.ToInt64(singal);

            Icebreaker.Models.IcebreakerEntities1 db = new IcebreakerEntities1();
            List<Icebreaker.Models.ScoreView> List = db.ScoreViews.Where(p => p.Singal == Singal ).ToList();
            string data = null;
            foreach (var a in List)
            {
                data = a.Text;
            }
            string Data = data;
            dt = gs.GetScoreTable(Data);
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


            int Count = dt.Rows.Count;                  //获取成绩报表总行数

            ScoreTools tool = new ScoreTools();
            int PassClassOfBottmScoreCount = 0;
            int NotPassClassCount = 0;
            int GPA = 0;
            string BottmScore = null;
            string TopScore = null;
            string AverageScore = null;
            
            PassClassOfBottmScoreCount = tool.GetPassClassOfBottmScoreCount(Count, dt); //获取低分飘过科目的总数
            NotPassClassCount= tool.GetNotPassClassCount(Count, dt);//获取未及格科目的数量
            GPA = tool.GetTopGPACount(Count, dt);//获取GPA大于等于4.7的数量
            BottmScore = tool.GetBottmScore(Count, dt);//获取最低成绩科目
            TopScore = tool.GetTopScore(Count, dt);//获取最高成绩科目
            AverageScore = tool.GetAverageScore(Count, dt);//获取个学期平均成绩

            ViewBag.PassClassOfBottmScoreCount = PassClassOfBottmScoreCount;
            ViewBag.NotPassClassCount = NotPassClassCount;
            ViewBag.GPA = GPA;
            ViewBag.BottmScore = BottmScore;
            ViewBag.TopScore = TopScore;
            ViewBag.AverageScore = AverageScore;

            return View();
        }
    }
}