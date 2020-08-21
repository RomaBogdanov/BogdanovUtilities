using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchDataInDataBases
{
    class Program
    {
        static void Main(string[] args)
        {
            SearcherInDB searcher = new SearcherInDB();
            //searcher.ConnectionString = "Data Source=KVTDECLSQL2;" +
            //    "User=dbadmin;Password=nhbnjgjkz;";
            searcher.ConnectionString = "Data Source=srv-sql-01;" +
    "User=dbadmin;Password=nhbnjgjkz;";
            //searcher.SearchValuesInFieldsDB("cmn.13029", "gtd2012_noblob");
            searcher.SearchValuesInFieldsServer("cmn.13029");


            //searcher.ConnectionString = "Data Source=KVTDECLSQL2;" +
            //    "Initial Catalog=ito;User=dbadmin;Password=nhbnjgjkz";
            //searcher.SearchValuesInFieldsDB("polo");
            foreach (var item in searcher.ColumnWithDatas)
            {
                Console.WriteLine($"{item.DataBase}\t {item.Table}\t " +
                    $"{item.Column}\t {item.RowsCount}");
            }
            Console.ReadLine();
        }
    }
}
