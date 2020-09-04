using SqlAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Data.SqlClient;

namespace SqlAnalyzer.Models
{
    public class SearchColumnsServerModel : SearchColumnsAbstractModel
    {
        public SearchColumnsServerModel()
        {
            ConnectionString = "Data Source=KVTDECLSQL2;" +
                        "User=dbadmin;Password=nhbnjgjkz;";
        }

        public override void SearchColumns()
        {
            SearchColumnsInServer();
            ColumnsCount();
        }

        /// <summary>
        /// Процедура поиска основных данных по всем колонкам сервера БД.
        /// </summary>
        private void SearchColumnsInServer()
        {
            Columns = new ObservableCollection<Column>();

            IsSearchingNow = true;
            //Task.Run(() =>
            //{
                //App.Current.Dispatcher.Invoke(() => IsSearchingNow = true);
                string query = "select name from sys.databases " +
        "where name not in ('master', 'tempdb', 'model', 'msdb')";
                using (SearchServerContext db = new SearchServerContext(ConnectionString))
                {
                    //TODO: здесь не хватает объединённого списка всех таблиц
                    var dbs = db.Database.SqlQuery<DataBaseInServer>(query).ToList();
                    foreach (var item in dbs)
                    {
                        SearchColumnsInDb(item.Name);
                    }
                }
                //App.Current.Dispatcher.Invoke(() => IsSearchingNow = false);
                IsSearchingNow = false;
            //});
        }

        /// <summary>
        /// Процедура поиска основных данных по всем колонкам конкретной БД на сервере.
        /// </summary>
        /// <param name="database"></param>
        private void SearchColumnsInDb(string database)
        {
            string query = "select TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, " +
                $"DATA_TYPE from {database}.INFORMATION_SCHEMA.COLUMNS";
            using (SearchServerContext db = new SearchServerContext(ConnectionString))
            {
                foreach (var item in db.Database.SqlQuery<Column>(query)
                    .AsEnumerable<Column>())
                {
                    App.Current.Dispatcher.Invoke(() => Columns.Add(item));
                }
            }
        }

        /// <summary>
        /// Сбор детализированой информации о конкретной колонке.
        /// </summary>
        /// <param name="column"></param>
        public override void DetailedInfoAboutColumn(Column column)
        {
            //Log log = new Log
            //{
            //    DB = column.TABLE_CATALOG,
            //    Table = column.TABLE_NAME,
            //    Col = column.COLUMN_NAME
            //};

            string command1 = $"Select top 1 [{column.COLUMN_NAME}] " +
                $"FROM [{column.TABLE_CATALOG}].[dbo].[{column.TABLE_NAME}]";
            string command2 = $"SELECT COUNT([{column.COLUMN_NAME}]) " +
                $"FROM[{column.TABLE_CATALOG}].[dbo].[{column.TABLE_NAME}]";
            string command3 = $"select COUNT([{column.COLUMN_NAME}]) from " +
                $"(SELECT distinct [{column.COLUMN_NAME}] " +
                $"FROM[{column.TABLE_CATALOG}].[dbo].[{column.TABLE_NAME}]) as T";

            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                try
                {

                    using (var query = new SqlCommand(command1, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                column.SampleValue = a.ToString();

                            }
                        }
                    }
                    using (var query = new SqlCommand(command2, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                column.CountRecsInColumn = Convert.ToInt64(a);
                            }
                        }
                    }
                    using (var query = new SqlCommand(command3, sc))
                    {
                        using (var res = query.ExecuteReader())
                        {
                            while (res.Read())
                            {
                                var a = res[0];
                                column.CountUniqueRecsInColumn = Convert.ToInt64(a);
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    column.IsError = true;
                    column.ErrorMsg = err.Message;
                }
            }

            //Samples.Add(log);
        }

        public override void SearchUniqueValuesInColumn()
        {
            Log log = SelectedLogColumn;
            UniqueValuesInColumn.Clear();
            string command1 = $"Select distinct [{SelectedLogColumn.Col}] " +
                $"FROM [{SelectedLogColumn.DB}].[dbo].[{SelectedLogColumn.Table}]";
            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                using (var query = new SqlCommand(command1, sc))
                {
                    using (var res = query.ExecuteReader())
                    {
                        while (res.Read())
                        {
                            var a = res[0];
                            //log.Value = a.ToString();
                            UniqueValuesInColumn.Add(new { A = a.ToString() });
                        }
                    }
                }
            }
        }
    }
}
