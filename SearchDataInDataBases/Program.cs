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
            Console.WriteLine("1 - начать поиск значения в таблицах сервера");
            Console.WriteLine("2 - начать поиск значения в таблицах конкретной базы данных");
            Console.WriteLine("3 - начать поиск частотности колонок на сервере");

            switch (Console.ReadLine())
            {
                case "1":
                    SearcherInDBServer searcher = new SearcherInDBServer();
                    searcher.ConnectionString = "Data Source=KVTDECLSQL2;" +
                        "User=dbadmin;Password=nhbnjgjkz;";
                    Console.Write("Введите строку поиска: ");
                    string searchStr = Console.ReadLine();
                    searcher.SearchValuesInFieldsServer(searchStr);
                    break;
                case "2":
                    Console.WriteLine("Функционал не реализован");
                    break;
                case "3":
                    SercherColumnNamesInDbServer searcher2 =
                        new SercherColumnNamesInDbServer();
                    searcher2.ConnectionString = "Data Source=KVTDECLSQL2;" +
                        "User=dbadmin;Password=nhbnjgjkz;";

                    searcher2.SearchCountOfColsInDbServer();
                    break;
                default:
                    Console.WriteLine("Выбрано неизвестное значение");
                    break;
            }




            //SearcherInDBServer searcher = new SearcherInDBServer();
            //searcher.ConnectionString = "Data Source=KVTDECLSQL2;" +
            //    "User=dbadmin;Password=nhbnjgjkz;";
    //        searcher.ConnectionString = "Data Source=srv-sql-01;" +
    //"User=dbadmin;Password=nhbnjgjkz;";
            //searcher.SearchValuesInFieldsDB("cmn.13029", "gtd2012_noblob");
            //searcher.SearchValuesInFieldsServer("cmn.13029");


            //searcher.ConnectionString = "Data Source=KVTDECLSQL2;" +
            //    "Initial Catalog=ito;User=dbadmin;Password=nhbnjgjkz";
            //searcher.SearchValuesInFieldsDB("polo");
            //foreach (var item in searcher.ColumnWithDatas)
            //{
            //    Console.WriteLine($"{item.DataBase}\t {item.Table}\t " +
            //        $"{item.Column}\t {item.RowsCount}");
            //}
            Console.ReadLine();
        }
    }
}
