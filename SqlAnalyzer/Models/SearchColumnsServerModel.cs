using SqlAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace SqlAnalyzer.Models
{
    public class SearchColumnsServerModel : SearchColumnsAbstractModel
    {
        public SearchColumnsServerModel()
        {
            ConnectionString = "Data Source=KVTDECLSQL2;" +
                        "User=dbadmin;Password=nhbnjgjkz;";
        }

        public override void SeachColumns()
        {
            //SearchColumnsInDb("DCL");
            /*string query = "select TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, " +
                "DATA_TYPE from INFORMATION_SCHEMA.COLUMNS";
            using (SearchDbContext db = new SearchDbContext(ConnectionString))
            {
                Columns = new ObservableCollection<Column>(
                    db.Database.SqlQuery<Column>(query).AsEnumerable<Column>());
            }*/

            SearchColumnsInServer();
            //Test();
            //SearchColumnsInDb("DCL");

        }

        private void Test()
        {
            string query = "select name from sys.databases " +
                "where name not in ('master', 'tempdb', 'model', 'msdb')";
            using (SearchServerContext db = new SearchServerContext(ConnectionString))
            {
                var dbs = db.Database.SqlQuery<DataBaseInServer>(query).ToList();
            }
        }

        private void SearchColumnsInServer()
        {
            Columns = new ObservableCollection<Column>();
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => IsSearchingNow = true);
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
                App.Current.Dispatcher.Invoke(() => IsSearchingNow = false);
            });
        }

        private void SearchColumnsInDb(string database)
        {
            string query = "select TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, " +
                $"DATA_TYPE from {database}.INFORMATION_SCHEMA.COLUMNS";
            using (SearchServerContext db = new SearchServerContext(ConnectionString))
            {
                //Columns = new ObservableCollection<Column>(
                //    db.Database.SqlQuery<Column>(query).AsEnumerable<Column>());

                foreach (var item in db.Database.SqlQuery<Column>(query)
                    .AsEnumerable<Column>())
                {
                    App.Current.Dispatcher.Invoke(() => Columns.Add(item));
                }
            }
        }
    }
}
