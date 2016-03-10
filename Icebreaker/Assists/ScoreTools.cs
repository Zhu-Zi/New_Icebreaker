using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Icebreaker.Assists
{
    /// <summary>
    /// 处理获得的成绩
    /// </summary>
    public class ScoreTools
    {
        /// <summary>
        /// 根据成绩计算个学期平均成绩
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetAverageScore(int count, DataTable dt)
        {
            int[] Semester = { 0, 0, 0, 0, 0, 0, 0, 0 };//学期数组，用于存取成绩表中的学期下标
            int Semester_i = 0;
            double[] SemesterAverageScore = new double[8];//学期平均成绩数组，记录各学期平均成绩
            int t = 1;                                    //学期计数器
            string StrSemesterAverageScore = null;//用于存取平均成绩

            for (int i = 0; i < count; i++)
            {
                if (dt.Rows[i][3].ToString().Equals("校公共选修课"))
                {
                    dt.Rows.Remove(dt.Rows[i]);
                    count--;
                }
            }
            //count = count - t;          //更新删除校公选课后的行数

            for (int i = 0; i < count - 1; i++)      //根据学期号的差值确定不同学期的下标
            {
                int sum = Convert.ToInt32(dt.Rows[i + 1][1]) - Convert.ToInt32(dt.Rows[i][1]);
                if (sum != 0)
                {
                    Semester[Semester_i] = i;
                    Semester_i++;
                }
                else if (i == count - 2)
                {
                    Semester[Semester_i] = i + 1;
                    Semester_i++;
                }
            }

            SemesterAverageScore[0] = GetAverageScoreHelper(0, Semester[0], dt);//第一学期平均成绩
            for (int i = 1; i < 8; i++)
            {
                if (Semester[i] == 0)
                {
                    break;
                }
                else
                {
                    SemesterAverageScore[i] = GetAverageScoreHelper(Semester[i - 1] + 1, Semester[i], dt);
                    t++;
                }
            }

            StrSemesterAverageScore = Convert.ToInt32(SemesterAverageScore[0]).ToString();
            for (int i = 1; i < t; i++)
            {
                StrSemesterAverageScore = StrSemesterAverageScore + ',' + Convert.ToInt32(SemesterAverageScore[i]).ToString();
            }


            return StrSemesterAverageScore;
        }

        /// <summary>
        /// 学期平均成绩计算方法
        /// </summary>
        /// <param name="First_i">学期开始下标</param>
        /// <param name="Last_i">学期结束下标</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public double GetAverageScoreHelper(int First_i, int Last_i, DataTable dt)
        {
            double ScoreSum = 0;
            double AverageScore = 0;
            int first_i = First_i;
            for (int i = 0; i <= (Last_i - First_i); i++)
            {
                ScoreSum = ScoreSum + Convert.ToInt32(dt.Rows[first_i][7]);
                first_i++;
            }
            AverageScore = ScoreSum / (Last_i - First_i + 1);

            return AverageScore;
        }
        /// <summary>
        /// 根据成绩获取最高成绩
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetTopScore(int count, DataTable dt)
        {

            Icebreaker.Models.ViewModelList vb = new Models.ViewModelList();
            List<ViewModels.ViewScoreTable> list = vb.TableToEntity<ViewModels.ViewScoreTable>(dt);

            list.Sort(delegate(ViewModels.ViewScoreTable small, ViewModels.ViewScoreTable big) { return Convert.ToInt32(small.Score) - Convert.ToInt32(big.Score); });//list通过委托排序


            string str = null;
            foreach (var t in list)
            {
                str = t.Course + "  " + t.Score ;
            }
            string result = str;

            return result;
        }

        /// <summary>
        /// 根据成绩获取最低成绩
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetBottmScore(int count, DataTable dt)
        {
            /// <summary>
            /// 通过对list的排序找到最低成绩
            /// </summary>
            //Icebreaker.Models.ViewModelList vb = new Models.ViewModelList();
            //List<ViewModels.ViewScoreTable> list = vb.TableToEntity<ViewModels.ViewScoreTable>(dt);

            //list.Sort(delegate(ViewModels.ViewScoreTable small, ViewModels.ViewScoreTable big) { return Convert.ToInt32(big.Score) - Convert.ToInt32(small.Score); });//list通过委托排序

            //string str = null;
            //foreach (var t in list)
            //{
            //    str = t.Course+ t.Score+ "|";
            //}
            //string result = str;

            ////<summary>
            ////方法二：通过对Datatable的排序来找到最低成绩
            ////<summary>
            DataView dv = new DataView(dt);
            dv.Sort = "Score asc";//升序排序
            DataTable DT = new DataTable();
            DT = dv.ToTable();
            string result = DT.Rows[0][2].ToString() + "  " + DT.Rows[0][7].ToString();

            return result;
        }

        /// <summary>
        /// 根据成绩获取高绩点的数量（GPA）
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetTopGPACount(int count, DataTable dt)
        {
            int t = 0;//计数器
            for (int i = 0; i < count; i++)
            {
                double GPA = Convert.ToDouble(dt.Rows[i][6].ToString());
                if (GPA >= 4.7)
                {
                    t++;
                }
            }

            return t;
        }

        /// <summary>
        /// 根据成绩获取不及格科目的数量
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetNotPassClassCount(int count, DataTable dt)
        {
            int t = 0;//计数器
            for (int i = 0; i < count; i++)
            {
                int FirstScore = Convert.ToInt32(dt.Rows[i][7].ToString());
                int SecoudScore = 60;                                             //补考成绩
                if (dt.Rows[i][8] != null && dt.Rows[i][8].ToString().Length != 0)
                {
                    SecoudScore = Convert.ToInt32(dt.Rows[i][8].ToString());
                }
                if (FirstScore < 60 && SecoudScore < 60)
                {
                    t++;
                }
            }

            return t;
        }

        /// <summary>
        /// 根据成绩获取低分飘过的科目数量
        /// </summary>
        /// <param name="count"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetPassClassOfBottmScoreCount(int count, DataTable dt)
        {
            int t = 0;//计数器
            for (int i = 0; i < count; i++)
            {
                int score = Convert.ToInt32(dt.Rows[i][7].ToString());
                if (((score - 60) >= 0) && ((score - 60) <= 3))
                {
                    t++;
                }
            }

            return t;
        }



    }//主类尾括号
}