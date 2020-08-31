using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SearchDataInDataBases
{
    /// <summary>
    /// Содержит процедуры поиска названий колонок на сервере базы данных
    /// </summary>
    /// <remarks>
    /// Должен содержать функционал:
    /// 1. Нахождение частоты названий колонок в базе данных;
    /// 2. Нахождение частоты названий колонок на сервере базы данных;
    /// 3. Вывод информации по названиям колонок в файл.
    /// </remarks>
    public class SercherColumnNamesInDbServer
    {
        private string connectionString;

        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        public string ConnectionString
        {
            get => connectionString;
            set => connectionString = value;
        }

        Dictionary<string, int> generalListOfColumns = new Dictionary<string, int>();

        public IEnumerable<ColumnData> SearchCountOfColsInDB(string dataBase)
        {
            string query = "select COLUMN_NAME, COUNT(COLUMN_NAME) as CountOfCols " +
                $"from [{dataBase}].INFORMATION_SCHEMA.COLUMNS group by COLUMN_NAME " +
                "order by CountOfCols desc";
            IEnumerable<ColumnData> cols = null;
            using (SearchDbContext db = new SearchDbContext(ConnectionString))
            {
                //TODO: Здесь сделать так, чтобы список сохранялся, а не был в виде динамического.
                cols = db.Database.SqlQuery<ColumnData>(query).AsEnumerable<ColumnData>();
                foreach (var item in cols)
                {
                    Console.WriteLine($"{item.COLUMN_NAME}\t{item.CountOfCols}");
                    if (generalListOfColumns.ContainsKey(item.COLUMN_NAME))
                    {
                        generalListOfColumns[item.COLUMN_NAME] += item.CountOfCols;
                    }
                    else
                    {
                        generalListOfColumns.Add(item.COLUMN_NAME, item.CountOfCols);
                    }
                }
            }
            return cols;
        }

        public void SearchCountOfColsInDbServer()
        {
            generalListOfColumns.Clear();
            string query = "select name from sys.databases " +
                "where name not in ('master', 'tempdb', 'model', 'msdb')";
            using (SearchDbContext db = new SearchDbContext(ConnectionString))
            {
                //TODO: здесь не хватает объединённого списка всех таблиц
                var dbs = db.Database.SqlQuery<DataBaseInServer>(query);
                foreach (var item in dbs)
                {
                    Console.WriteLine($"Исследуем базу данных {item.Name}");
                    SearchCountOfColsInDB(item.Name);
                }
            }
            Console.WriteLine("Интегральный результат:");
            foreach (var item in generalListOfColumns.OrderByDescending(p => p.Value))
            {
                Console.WriteLine($"{item.Key}\t\t\t{item.Value}");
                File.AppendAllText("AllColumnsInServer.txt",
                    $"{item.Key}\t\t\t{item.Value}" +
                    Environment.NewLine);
            }
        }
    }

    public class SearchDbContext : DbContext
    {
        public DbSet<ColumnData> ColumnDatas { get; set; }
        public DbSet<DataBaseInServer> DataBasesInServer { get; set; }

        public SearchDbContext(string connectionString) : base(connectionString)
        {

        }
    }

    public class ColumnData
    {
        [Key]
        public string COLUMN_NAME { get; set; }
        public int CountOfCols { get; set; }
    }

    public class DataBaseInServer
    {
        [Key]
        public string Name { get; set; }
    }
}
