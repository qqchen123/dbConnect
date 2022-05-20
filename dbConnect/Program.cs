using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace dbConnect
{
    class Program
    {
        static string conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
        static void Main(string[] args)
        {
            MyTrans();
        }

        public static void MyTrans()
        {
            using (SqlConnection conn = new SqlConnection(Program.conStr))
            {
                SqlTransaction sqlTransaction = null;
                try
                {
                    conn.Open();
                    sqlTransaction = conn.BeginTransaction();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.Transaction = sqlTransaction;
                    cmd.CommandText = "insert into class (classname,stunumber) values(@classname,@stunumber);select @@identity";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@classname", "初三2"));
                    cmd.Parameters.Add(new SqlParameter("@stunumber", "123"));
                    object oId = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    int classId = 0;
                    if (oId != null)
                    {
                        classId = int.Parse(oId.ToString());
                    }
                    cmd.CommandText = "insert into student (username,email,address,class_id) values(@username,@email,@address,@class_id)";
                    cmd.Parameters.Clear();
                    SqlParameter[] para = {
                        new SqlParameter("@username","sunsun"),
                        new SqlParameter("@email","sunsun@qq.com"),
                        new SqlParameter("@address","guangzhou"),
                        new SqlParameter("@class_id",classId)
                    };
                    cmd.Parameters.AddRange(para);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    sqlTransaction.Commit();
                    Console.WriteLine("用户添加成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine("回滚");
                    Console.WriteLine(e.Message);
                    sqlTransaction.Rollback();
                }
            }
            Console.ReadKey();
        }

        public static void MyAdapterDataUpdataSelf()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            String sql = "select * from student;";
            SqlConnection sqlConnection = new SqlConnection(conStr);
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("student");
        }

        public static void MyAdapterDataUpdata()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            String sql = "select * from student;";
            SqlConnection sqlConnection = new SqlConnection(conStr);
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("student");
            da.Fill(dt);
            dt.Rows[1]["username"] = "testuser";

            DataRow dr = dt.NewRow();
            dr["username"] = "newnew1";
            dr["email"] = "new1@qq.com";
            dr["address"] = "nanjing1";
            dr["class_id"] = 1;
            dt.Rows.Add(dr);
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(da);
            da.Update(dt);

            Console.ReadKey();

        }

        public static void MyAdapterDataTable()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            String sql = "select * from student;";
            SqlConnection sqlConnection = new SqlConnection(conStr);
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable("student");
            da.Fill(dt);



            Console.ReadKey();
        }

        public static void MyAdapter2()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            //String sql = "select * from student;select * from class";
            String sql = "select * from student;";
            SqlConnection sqlConnection = new SqlConnection(conStr);
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();

            //多个结果集，填充映射
            //sqlDataAdapter1.TableMappings.Add("Table","student");
            //sqlDataAdapter1.TableMappings.Add("Table1","class");
            //sqlDataAdapter1.Fill(ds);

            //单个结果集，填充映射
            sqlDataAdapter1.Fill(ds, "student");



            Console.ReadKey();

        }

        public static void MyAdapter()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            String sql = "select * from student";
            SqlConnection sqlConnection = new SqlConnection(conStr);

            //1.方式一：设置SelectCommand
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = new SqlCommand(sql, sqlConnection);
            //2.方式二：通过一个SQL Command对象来实例化一个adapter
            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter(sqlCommand);
            //3.方式三：通过查询语句和连接对象
            SqlDataAdapter sqlDataAdapter2 = new SqlDataAdapter(sql, sqlConnection);
            //4.方式四：通过查询语句和连接字符串
            SqlDataAdapter sqlDataAdapter3 = new SqlDataAdapter(sql, conStr);

        }

        public static void MyDataRelation()
        {
            DataSet ds = new DataSet("ds");
            DataTable dt1 = new DataTable("userinfo");
            DataTable dt2 = new DataTable("class");
            ds.Tables.Add(dt1);
            dt1.Columns.Add("id", typeof(int));
            dt1.Columns.Add("username", typeof(string));
            dt1.Columns.Add("email", typeof(string));
            dt1.Columns.Add("address", typeof(string));
            dt1.Columns.Add("class_id", typeof(int));
            ds.Tables.Add(dt2);
            dt2.Columns.Add("id", typeof(int));
            dt2.Columns.Add("classname", typeof(string));
            dt2.Columns.Add("stunumber", typeof(int));
            //唯一约束
            // dt2.Constraints.Add(new UniqueConstraint(dt2.Columns[1]));
            //外键约束
            // dt1.Constraints.Add(new ForeignKeyConstraint(dt2.Columns[0], dt1.Columns[4]));
            //默认情况：建立关系，就自动为父表建立唯一约束，子表中外键建立一个外键约束
            DataRelation dataRelation = new DataRelation("myrelation", dt2.Columns[0], dt1.Columns[4]);
            ds.Relations.Add(dataRelation);
            InitData(dt1, dt2);
            //使用关系
            foreach (DataRow row in dt1.Rows)
            {
                Console.WriteLine(
                    $"id:{row[0].ToString()},username:{row[1].ToString()},email:{row[2].ToString()},address:{row[3].ToString()},class_id:{row[4].ToString()}");
                DataRow[] parentRows = row.GetParentRows(dataRelation);
                foreach (DataRow parentRow in parentRows)
                {
                    Console.WriteLine(
                        $"id:{parentRow[0].ToString()},classname:{parentRow[1].ToString()},stunumber:{parentRow[2].ToString()}");
                }
            }

            Console.WriteLine("----------------------------------------------");

            foreach (DataRow row in dt2.Rows)
            {
                Console.WriteLine(
                    $"id:{row[0].ToString()},classname:{row[1].ToString()},stunumber:{row[2].ToString()}"
                );

                DataRow[] rows = row.GetChildRows(dataRelation);
                foreach (DataRow dataRow in rows)
                {
                    Console.WriteLine(
                        $"id:{dataRow[0].ToString()}," +
                        $"username:{dataRow[1].ToString()}," +
                        $"email:{dataRow[2].ToString()}," +
                        $"address:{dataRow[3].ToString()}," +
                        $"class_id:{dataRow[4].ToString()}"
                    );
                }
            }

            Console.ReadKey();
        }

        private static void InitData(DataTable dt1, DataTable dt2)
        {
            DataRow dr2 = dt2.NewRow();
            dr2["id"] = 1;
            dr2["classname"] = "初一";
            dr2["stunumber"] = 1;
            dt2.Rows.Add(dr2);
            dr2 = dt2.NewRow();
            dr2["id"] = 2;
            dr2["classname"] = "初二";
            dr2["stunumber"] = 2;
            dt2.Rows.Add(dr2);
            dr2 = dt2.NewRow();
            dr2["id"] = 3;
            dr2["classname"] = "初三";
            dr2["stunumber"] = 3;

            DataRow dr1 = dt1.NewRow();
            dr1["id"] = 1;
            dr1["username"] = "qqchen";
            dr1["email"] = "qqchen@qq.com";
            dr1["address"] = "bj";
            dr1["class_id"] = 2;
            dt1.Rows.Add(dr1);
            dr1 = dt1.NewRow();
            dr1["id"] = 2;
            dr1["username"] = "qqchen2";
            dr1["email"] = "qqchen2@qq.com";
            dr1["address"] = "bj2";
            dr1["class_id"] = 1;
            dt1.Rows.Add(dr1);
            dr1 = dt1.NewRow();
            dr1["id"] = 3;
            dr1["username"] = "qqchen3";
            dr1["email"] = "qqchen3@qq.com";
            dr1["address"] = "bj3";
            dr1["class_id"] = 1;
            dt1.Rows.Add(dr1);
        }


        public static void MyDataSet()
        {
            //默认名称 newdataset
            DataSet ds = new DataSet("ds1");
            //常用属性
            //datasetname

            DataTable dt1 = new DataTable("dt1");
            ds.Tables.Add(dt1); //将dt1添加到ds中

            //获取表
            DataTable dt2 = ds.Tables[0];

            //ds.Relations.Add() //添加关系到ds

            //常用方法
            ds.AcceptChanges(); // 提交
            ds.RejectChanges(); //回滚

            Console.ReadKey();
        }

        public static void MyDataTable()
        {
            DataTable dt = new DataTable("userinfo");
            //字段 主键  约束
            DataColumn dc = new DataColumn();
            dc.ColumnName = "id";
            dc.DataType = typeof(int);
            dt.Columns.Add(dc);
            dt.Columns.Add("username", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("address", typeof(string));
            dt.Columns.Add("class_id", typeof(int));
            //设置主键
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
            //设置唯一约束
            dt.Constraints.Add(new UniqueConstraint(dt.Columns[1]));
            //创建一行，并与dt具有相同的架构
            DataRow dr = dt.NewRow();
            dr[0] = 1;
            dr["username"] = "admin";
            dr["email"] = "admin@163.com";
            dr["address"] = "songjiang";
            dr["class_id"] = 2; //RowState = Detached
            //将数据添加到dt表中,rowstate=added
            dt.Rows.Add(dr);
            //dt.RejectChanges();//回滚
            //提交更改 unchanged
            dt.AcceptChanges();
            //修改 modified 已修改
            dr["username"] = "admin123";
            dt.AcceptChanges(); //unchanged
            //dr.Delete();//deleted
            //dt.AcceptChanges();//RowState = Detached
            //dt.Rows.Remove(dr); // RowState = Detached;remove 相当于  delete + accepchanges
            //datarow  --rowstate:detached孤立存在
            //DataRowState
            //dt.Clear();//清空数据，架构还在
            DataTable dt2 = dt.Copy(); //复制架构和数据

            DataRow dr2 = dt2.NewRow();
            dr2[0] = 2;
            dr2["username"] = "admin2";
            dr2["email"] = "admin2@163.com";
            dr2["address"] = "songjiang2";
            dr2["class_id"] = 22; //RowState = Detached
            dt2.Rows.Add(dr2);

            DataTable dt3 = dt.Clone(); //只复制架构，不包含数据

            //将dt2数据合并到dt中
            dt.Merge(dt2);

            //获取所有的行
            DataRow[] rows = dt.Select();
            DataRow[] rows2 = dt.Select("id>1");

            Console.ReadKey();
        }


        public static void SqlDataSet()
        {
            //1.创建
            DataSet dataSet = new DataSet("ds1");

            DataTable dt1 = new DataTable();
            //添加dt1到dataset中
            dataSet.Tables.Add(dt1);
            //获取表
            DataTable dt2 = dataSet.Tables[0];
            // 添加datarelation到dataset中
            //dataSet.Relations.Add()

            //常用方法--
            //提交
            dataSet.AcceptChanges();
            //回滚
            dataSet.RejectChanges();
            //清楚表中所有行的数据
            dataSet.Clear();

            dataSet.Copy();
            dataSet.Clone();

            //dataSet.Merge();
            dataSet.Reset();
            //dataSet.Load(IDataReader);
        }

        public static void SqlDataTable()
        {
            //创建datatable并赋表名
            DataTable dataTable = new DataTable("userInfo");
            //列 约束 主键
            DataColumn dataColumn = new DataColumn();
            //方式一：添加一列
            dataColumn.ColumnName = "id";
            dataColumn.DataType = typeof(int);
            dataTable.Columns.Add(dataColumn);

            //方式二：添加一列（添加字段）
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("address", typeof(string));
            //设置主键
            dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };
            //设置唯一约束
            dataTable.Constraints.Add(new UniqueConstraint(dataTable.Columns[1]));
            //架构定义好了后，添加数据

            // 具有相同的架构
            DataRow dataRow = dataTable.NewRow();
            dataRow[0] = 1001;
            dataRow["username"] = "cej";
            dataRow["email"] = "ejchen@qq.com";
            dataRow["address"] = "shandong";
            //这条数据并没有添加到datatable中，状态为detached


            //添加到datatable中，状态为added
            dataTable.Rows.Add(dataRow);

            //回滚上面的操作
            //dataTable.RejectChanges();

            //提交更改，状态为unchanged
            dataTable.AcceptChanges();

            //修改，modified,已修改
            dataRow["address"] = "shandong2";

            //状态为 unchanged
            dataTable.AcceptChanges();

            //状态为deleted，已删除
            //dataRow.Delete();

            //状态为 ？ detached
            //dataTable.AcceptChanges();

            // 状态为 detached
            //dataTable.Rows.Remove(dataRow);


            //清空数据，架构还在
            //dataTable.Clear();
            //复制架构和数据
            DataTable dataTable2 = dataTable.Copy();

            //只复制架构，不复制数据
            DataTable dataTable3 = dataTable.Clone();


            DataRow dataRow2 = dataTable2.NewRow();
            dataRow2[0] = 1002;
            dataRow2["username"] = "cej2";
            dataRow2["email"] = "ejchen2@qq.com";
            dataRow2["address"] = "shan2dong";
            dataTable2.Rows.Add(dataRow2);
            //将datatable2添加到datatable中
            dataTable.Merge(dataTable2);
        }


        public static void SqlCommandParam()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
                using (connection)
                {
                    connection.Open();
                    string sql = "select * from student where username=@username";
                    SqlCommand sqlCommand = new SqlCommand(sql, connection);
                    //添加单个参数使用；
                    //sqlCommand.Parameters.AddWithValue("@username","xiaoming");

                    //添加多个参数时使用；
                    SqlParameter[] para =
                    {
                        new SqlParameter("@username", "zhangsan"),
                        new SqlParameter("@email", "zhangsan@qq.com")
                    };
                    sqlCommand.Parameters.AddRange(para);
                    SqlDataReader res = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (res.Read())
                    {
                        int id = int.Parse(res["id"].ToString());
                        string username = res["username"].ToString();
                        string email = res["email"].ToString();
                        string address = res["address"].ToString();
                        Console.WriteLine($"id {id},username {username},email {email}, address {address}", id, username,
                            email, address);
                    }
                    //res.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        public static void SqlParam()
        {
            new SqlParameter();
        }

        //适合读取数据量小的情况，数据量大的话会占用连接；
        public static void SqlExcuteReader()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
                using (connection)
                {
                    connection.Open();
                    string sql = "select * from student";
                    SqlCommand sqlCommand = new SqlCommand(sql, connection);
                    //查询返回一个对象，sqldatareader 数据流
                    //关闭res并没有释放conn
                    //SqlDataReader res = sqlCommand.ExecuteReader();
                    //不管是关闭res，还是conn，他们都会一致关闭
                    SqlDataReader res = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (res.Read())
                    {
                        int id = int.Parse(res["id"].ToString());
                        string username = res["username"].ToString();
                        string email = res["email"].ToString();
                        string address = res["address"].ToString();
                        Console.WriteLine($"id {id},username {username},email {email}, address {address}", id, username,
                            email, address);
                    }
                    //res.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


        public static void SqlExcuteScale()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            SqlConnection connection = new SqlConnection(conStr);
            try
            {
                using (connection)
                {
                    connection.Open();
                    string sql = "select count(*) from student";
                    //string sql = "insert into student (username,email,address,class_id) values ('xiaoming3','xm2@qq.com','shanghai2',2);select @@identity";
                    SqlCommand sqlCommand = new SqlCommand(sql, connection);
                    //查询操作返回一行一列，适用于统计
                    object res = sqlCommand.ExecuteScalar();
                    if (res != null)
                    {
                        Console.WriteLine(res);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        public void SqlServerTest1()
        {
            //1.创建连接
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "server=.;database=userinfo;uid=sa;pwd=123123";
            //connection.Database
            //connection.DataSource
            //connection.State
            //connection.ConnectionTimeout
            //2.打开连接
            connection.Open();
            //3.创建执行命令对象
            connection.CreateCommand();

            //4.关闭连接
            connection.Close();
            //5.释放连接
            //connection.Dispose();
        }

        public static void SqlServerByConfigFile()
        {
            SqlConnection connection = null;
            try
            {
                String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
                connection = new SqlConnection(conStr);
                connection.Open();
                //Console.WriteLine($"Data Source: {connection.DataSource}");
                //Console.WriteLine($"Data Base: {connection.Database}");
                //Console.WriteLine($"Server version: {connection.ServerVersion}");
                //Console.WriteLine($"State: {connection.State}");
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public static void SqlServerByConfigFileUsing()
        {
            SqlConnection connection = null;
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            connection = new SqlConnection(conStr);

            try
            {
                using (connection)
                {
                    connection.Open();
                    //string sql = "select * from student";
                    string sql =
                        "insert into student (username,email,address,class_id) values ('xiaoming','xm@qq.com','shanghai',1)";
                    SqlCommand sqlCommand = new SqlCommand(sql, connection);
                    int num = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void MaxConnectPool()
        {
            String conStr = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
            for (int i = 0; i < 10; i++)
            {
                SqlConnection connection = new SqlConnection(conStr);
                connection.Open();
                Console.WriteLine($"第{i + 1}个连接一打开！");
            }
        }
    }
}