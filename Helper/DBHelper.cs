using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
//using Model;
using System.ComponentModel;

namespace Helper
{
    public class DBHelper
    {
        private static string constr = @"Data Source=.\SQLEXPRESS;Initial Catalog=MyBookDB;User ID=sa;Password=123456;TrustServerCertificate=True;Encrypt=False";
        //private static BindingList<BookInfo> BindingbookList = new BindingList<BookInfo>();

        public static int Count;//定义一个静态属性来存储数据总数
        public static int PageSize = 9;//定义一个静态属性来存储每页显示的数据条数
        public static int PageBegin = 1;//定义一个静态属性来存储当前页的起始索引
        public static int PageCount = 0;//定义一个静态属性来存储总页数


        #region  图书实例
        ///// <summary>
        ///// 初始化
        ///// </summary>
        //public static void InitData()
        //{
        //    SqlConnection con = new SqlConnection(constr);
        //    con.Open();
        //    SqlCommand cmd = new SqlCommand("SELECT * FROM Book", con);
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    BindingList<BookInfo> bookList = new BindingList<BookInfo>();
        //    while (reader.Read())
        //    {
        //        BookInfo book = new BookInfo();
        //        book.id = (int)reader["id"];
        //        book.bookName = (string)reader["Title"];
        //        book.price = (decimal)reader["Price"];
        //        bookList.Add(book);
        //    }
        //    reader.Close();
        //    con.Close();
        //    BindingbookList = bookList;
        //    Count = bookList.Count; // 更新总数
        //    PageCount = Count == 0 ? 0 : (int)Math.Ceiling((double)Count / PageSize); // 重新计算总页数
        //}

        ///// <summary>
        ///// 获取数据库中的书籍列表
        ///// </summary>
        ///// <returns></returns>
        //public static BindingList<BookInfo> GetBookList()
        //{
        //    InitData();
        //    return BindingbookList;
        //}

        #endregion



        /// <summary>
        /// 增删改通用SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static int CommandSQL(string strSQL)
        {
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = strSQL;
            int num = cmd.ExecuteNonQuery();
            conn.Close();
            return num;
        }




        /// <summary>
        /// 增删改通用，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramArr"></param>
        /// <returns></returns>
        //SqlParameter：可以通过名称或位置来引用该参数。防止SQL注入攻击，建议使用参数化查询来执行SQL语句。
        public static int ExecuteNonQuery(string sql, SqlParameter[] paramArr, bool isProc)
        {
            using (SqlConnection conn = new SqlConnection(constr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    //根据isProc参数来设置CommandType属性，是否执行存储过程，还是执行普通查询
                    cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;
                    if (paramArr != null)
                    {
                        foreach (SqlParameter param in paramArr)
                        {
                            cmd.Parameters.Add(param);// 添加参数
                        }

                    }
                    int num = cmd.ExecuteNonQuery();
                    return num;
                }
            }
        }



        /// <summary>
        /// 查询通用，返回DataTable,判断是否执行存储过程，如果isProc为true，则执行存储过程，否则执行普通查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramArr"></param>
        /// <param name="isProc"></param>
        /// <returns></returns>
        public static DataTable GetDataTablList(string sql, SqlParameter[] paramArr, bool isProc)
        {
            DataTable dt = new DataTable();
            //使用using语句来确保对象在使用完毕后正确释放资源
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //根据isProc参数来设置CommandType属性，
                    //如果isProc为true，则设置为CommandType.StoredProcedure，否则设置为CommandType.Text
                    cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;

                    //判断参数数组是否为null
                    if (paramArr != null)
                    {
                        //使用foreach循环来遍历参数数组，并将每个参数添加到SqlCommand对象的Parameters集合中
                        foreach (var param in paramArr)
                        {
                            cmd.Parameters.Add(param);// 添加参数
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        
                        adapter.Fill(dt);//填充DataTable

                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 查询通用，返回DataTable,判断是否执行存储过程，如果isProc为true，则执行存储过程，否则执行普通查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramArr"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, SqlParameter[] paramArr, bool isProc)
        {
            //使用using语句来确保对象在使用完毕后正确释放资源
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //根据isProc参数来设置CommandType属性，
                    //如果isProc为true，则设置为CommandType.StoredProcedure，否则设置为CommandType.Text
                    cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;

                    //判断参数数组是否为null
                    if (paramArr != null)
                    {
                        //使用foreach循环来遍历参数数组，并将每个参数添加到SqlCommand对象的Parameters集合中
                        foreach (var param in paramArr)
                        {
                            cmd.Parameters.Add(param);// 添加参数
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// 查询通用，查询单一值（如聚合结果）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, SqlParameter[] parameters, bool isProc)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    return cmd.ExecuteScalar();//返回查询结果的第一行第一列的值
                }
            }
        }

    }
}


