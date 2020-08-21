using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;

namespace SearchDataInDataBases
{
    /// <summary>
    /// Поисковик по конкретной базе данных.
    /// </summary>
    public class SearcherInDB
    {
        private string connectionString;
        private SqlConnection connection;

        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        public string ConnectionString
        {
            get => connectionString;
            set => connectionString = value;
        }

        public SqlConnection Connection
        { get => connection; set => connection = value; }

        public DataTable SearchColumns { get; set; } = new DataTable();

        public List<ColumnWithData> ColumnWithDatas { get; set; } =
            new List<ColumnWithData>();

        /// <summary>
        /// Исследование сервера баз данных на предмет наличия текста.
        /// </summary>
        public void SearchValuesInFieldsServer(string text)
        {
            string databasesListQuery = "select name from sys.databases " +
                "where name not in ('master', 'tempdb', 'model', 'msdb')";
            DataTable databases = new DataTable();
            Connection = new SqlConnection(ConnectionString);
            using (SqlDataAdapter da =
                new SqlDataAdapter(databasesListQuery, Connection))
            {
                da.Fill(databases);
                string oldConStr = ConnectionString;
                foreach (DataRow item in databases.Rows)
                {
                    Console.WriteLine($"Исследуем базу данных " +
                        $"{item.ItemArray.ElementAt(0)}");
                    ConnectionString = $"{oldConStr}" +
                        $"Initial Catalog={item.ItemArray.ElementAt(0)};";
                    //Console.WriteLine(ConnectionString);
                    SearchValuesInFieldsDB(text);
                }
            }

        }

        public void SearchValuesInFieldsDB(string text, string dbName)
        {
            string oldConStr = ConnectionString;
            ConnectionString = $"{oldConStr}" +
                        $"Initial Catalog={dbName};";
            SearchValuesInFieldsDB(text);
            ConnectionString = oldConStr;
        }

        /// <summary>
        /// Поиск колонок баз данных, которые содержат данное значение.
        /// </summary>
        /// <param name="isPartOfValue">Распространяется на строковые значения. 
        /// Означает, что значение может являться частью строки в столбце.</param>
        public void SearchValuesInFieldsDB(string text, bool isPartOfValue = true)
        {
            DateTime begin = DateTime.Now;
            //ColumnWithDatas.Clear();
            //SearchColumns.Columns.Add("Повторений", typeof(int));
            string columnsInCatalogQuery = "select TABLE_CATALOG, TABLE_NAME, " +
                "COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS";
            SearchColumns.Clear();
            Connection = new SqlConnection(ConnectionString);
            using (SqlDataAdapter da = new SqlDataAdapter(
                columnsInCatalogQuery, Connection))
            {
                da.Fill(SearchColumns);
                foreach (DataRow row in SearchColumns.Rows)
                {
                    SearchValuesInColumn(row, text, da);
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan workTime = end - begin;
            Console.WriteLine($"Время изучения каталога {workTime}");
        }

        private void SearchValuesInColumn(DataRow columnParams, string searchText, SqlDataAdapter dataAdapter)
        {
            Console.Write($"{DateTime.Now} Обрабатываем колонку {columnParams[2]}. ");

            string sqlQuery = $"select [{columnParams.ItemArray.ElementAt(2)}] " +
    $"from {columnParams.ItemArray.ElementAt(1)} " +
    $"where [{columnParams.ItemArray.ElementAt(2)}] like '%{searchText}%'";
            dataAdapter.SelectCommand = new SqlCommand(sqlQuery, Connection);
            DataTable dt = new DataTable();
            try
            {
                dataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    ColumnWithData columnWithData = new ColumnWithData
                    {
                        DataBase = columnParams.ItemArray.ElementAt(0).ToString(),
                        Table = columnParams.ItemArray.ElementAt(1).ToString(),
                        Column = columnParams.ItemArray.ElementAt(2).ToString(),
                        RowsCount = dt.Rows.Count
                    };

                    ColumnWithDatas.Add(columnWithData);

                    File.AppendAllText("Result.txt",
                        $"{columnWithData.DataBase} {columnWithData.Table} " +
                        $"{columnWithData.Column} {columnWithData.RowsCount}" + 
                        Environment.NewLine);
                }
                //columnParams[0] = dt.Rows.Count;
                Console.WriteLine($"Найдено {dt.Rows.Count} записей");
                //}
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

        }
    }

    public class ColumnWithData
    {
        public string DataBase { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public long RowsCount { get; set; }
    }
}
