using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FixChengeDataInDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string command = "Select max([Current LSN]) FROM sys.fn_dblog(null, null)";

            string dc = "Data Source=localhost;Initial Catalog=Test;Integrated Security=True";
            string maxLog = "";

            using (var sc = new SqlConnection(dc))
            {
                sc.Open();
                using (var query = new SqlCommand(command, sc))
                {
                    using (var res = query.ExecuteReader())
                    {
                        while (res.Read())
                        {
                            maxLog = res.GetString(0);//.GetSqlValue(0);
                        }
                    }
                }
            }

            bool isFinish = false;

            Task.Run(() =>
            {
                while (!isFinish)
                {
                    command = "select [Current LSN], Operation, AllocUnitName from sys.fn_dblog(null, null) " +
                        $"where Operation in ('LOP_INSERT_ROWS', 'LOP_MODIFY_ROW') and " +
                        $"[Current LSN] > '{maxLog}'";// order by [Current LSN]";
                    using (var sc = new SqlConnection(dc))
                    {
                        sc.Open();
                        using (var query = new SqlCommand(command, sc))
                        {
                            using (var res = query.ExecuteReader())
                            {
                                while (res.Read())
                                {
                                    maxLog = res.GetString(0);
                                    string op = res.GetString(1);
                                    string tab = res.GetString(2);
                                    Console.WriteLine($"{op} {tab}");
                                }
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(3000);
                }
            });
            Console.ReadLine();
            isFinish = true;
        }
    }
}
