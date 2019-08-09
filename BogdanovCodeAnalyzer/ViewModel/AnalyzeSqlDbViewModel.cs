using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace BogdanovCodeAnalyzer.ViewModel
{
    class AnalyzeSqlDbViewModel : NotifyPropertyChanged
    {

        private ObservableCollection<ConnectionDB> connections;
        private string valueToSearchInDbTabs;
        private System.Data.DataTable dT;
        private System.Data.DataView dV;


        public ObservableCollection<ConnectionDB> Connections
        {
            get => connections;
            set
            {
                connections = value;
                OnPropertyChanged();
            }
        }

        public System.Data.DataTable DT
        {
            get => dT;
            set
            {
                dT = value;
                OnPropertyChanged();
            }
        }

        public System.Data.DataView DV
        {
            get => dV;
            set
            {
                dV = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Значение для поиска в полях таблиц БД
        /// </summary>
        public string ValueToSearchInDbTabs
        {
            get => valueToSearchInDbTabs;
            set
            {
                valueToSearchInDbTabs = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда для поиска таблиц и столбцов в БД для поиска полей.
        /// </summary>
        public ICommand SearchValuesInFieldsDbCommand { get; set; }

        public AnalyzeSqlDbViewModel()
        {
            Connections = new ObservableCollection<ConnectionDB>();
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=gorizont;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=lab1;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            }); Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=seller;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=Sklad;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });


            SearchValuesInFieldsDbCommand = new RelayCommand(obj => SearchValuesInFieldsDb());
        }

        private void SearchValuesInFieldsDb()
        {
            List<string> strTypes = new List<string> { "char", "nchar", "nvarchar", "varchar", "timestamp", "ntext", "varbinary", "smalldatetime" };
            DT = null;
            foreach (var conStr in Connections)
            {
                if (!conStr.IsChecked)
                {
                    continue;
                }

                System.Data.DataTable dt = new System.Data.DataTable();
                string sqlQuery = "select TABLE_CATALOG, TABLE_NAME, " +
                    "COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS";

                System.Data.SqlClient.SqlConnection cn =
                    new System.Data.SqlClient.SqlConnection(conStr.ConnectionString);

                using (System.Data.SqlClient.SqlDataAdapter da =
                    new System.Data.SqlClient.SqlDataAdapter(sqlQuery, cn))
                {
                    da.Fill(dt);
                    if (string.IsNullOrEmpty(ValueToSearchInDbTabs) && DT == null)
                    {
                        DT = dt;
                        return;
                    }
                    if (DT == null)
                    {
                        DT = dt.Clone();// new System.Data.DataTable();
                        DT.Columns.Add("Повторений", typeof(int));

                    }
                    long resLong;
                    if (long.TryParse(ValueToSearchInDbTabs, out resLong))
                    {
                        System.Data.DataTable dt2 = new System.Data.DataTable();
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            if (strTypes.Contains(item[3].ToString()))
                            {
                                continue;
                            }
                            sqlQuery = $"select {item.ItemArray.ElementAt(2)} " +
                                $"from {item.ItemArray.ElementAt(1)} " +
                                $"where {item.ItemArray.ElementAt(2)} = {ValueToSearchInDbTabs}";


                            da.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlQuery, cn);
                            da.Fill(dt2);
                            int c = dt2.Rows.Count;
                            if (c > 0)
                            {

                                DT.Rows.Add(item.ItemArray.ElementAt(0), item.ItemArray.ElementAt(1), item.ItemArray.ElementAt(2), item.ItemArray.ElementAt(3), c);
                                //DT.Rows.Add(item.ItemArray, c);
                            }
                        }
                    }
                    else
                    {
                        System.Data.DataTable dt2 = new System.Data.DataTable();
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            if (strTypes.Contains(item[3].ToString()) && item[3].ToString() != "varbinary" && item[3].ToString() != "ntext" && item[3].ToString() != "varchar")
                            {
                                sqlQuery = $"select {item.ItemArray.ElementAt(2)} " +
                                    $"from {item.ItemArray.ElementAt(1)} " +
                                    $"where {item.ItemArray.ElementAt(2)} = '{ValueToSearchInDbTabs}'";


                                da.SelectCommand = new System.Data.SqlClient.SqlCommand(sqlQuery, cn);
                                da.Fill(dt2);
                                int c = dt2.Rows.Count;
                                if (c > 0)
                                {
                                    DT.Rows.Add(item.ItemArray.ElementAt(0), item.ItemArray.ElementAt(1), item.ItemArray.ElementAt(2), item.ItemArray.ElementAt(3), c);
                                }
                            }
                        }
                    }
                }
            }
            //DT = dt;
            //DT.Columns.Remove("DATA_TYPE");
            DV = DT.DefaultView;
        }
    }
}
