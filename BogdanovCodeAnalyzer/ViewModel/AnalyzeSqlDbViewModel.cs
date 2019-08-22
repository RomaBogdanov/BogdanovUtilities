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
    /// <summary>
    /// Обработка логики контрола для анализа БД.
    /// </summary>
    /// <remarks>
    /// Реализация функционала по взаимодействию с БД T-SQL:
    /// v------1. Уметь находить все таблицы с колонками в базе данных содержащие поле с данными;
    /// 1.1. Тоже самое, но с массивом полей данных;
    /// v------2. Уметь находить все таблицы с колонками в массиве баз данных содержащие поле с данными;
    /// 2.1. Тоже самое, но с массивом полей данных;
    /// 3. Исключать таблицы из рассмотрения;
    /// 4. Научиться логгировать изменения в БД;
    /// 5. Научиться делать и восстанавливать бэкапы.
    /// 6. Вынести список строк подключения в конфигурационный файл:
    /// 6.1. Уметь добавлять строку подключения;
    /// 6.2. Уметь удалять строку подключения;
    /// 6.3. Уметь редактировать строку подключения;
    /// </remarks>
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

        /// <summary>
        /// Команда для поиска таблиц и столбцов в БД для поиска таблиц.
        /// </summary>
        public ICommand SearchValuesInTablesDbCommand { get; set; }

        /// <summary>
        /// Команда для поиска таблиц и столбцов в БД для поиска колонок.
        /// </summary>
        public ICommand SearchValuesInColumnsDbCommand { get; set; }

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
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=dopsell;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=sintez;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });

            SearchValuesInFieldsDbCommand = new RelayCommand(obj => SearchValuesInFieldsDb());
            SearchValuesInTablesDbCommand = new RelayCommand(obj => SearchValuesInTablesDb());
            SearchValuesInColumnsDbCommand = new RelayCommand(obj => SearchValuesInColumnsDb());
        }

        private void SearchValuesInColumnsDb()
        {
            DT = null;
            foreach (var conStr in Connections)
            {
                if (!conStr.IsChecked)
                {
                    continue;
                }

                System.Data.DataTable dt = new System.Data.DataTable();
                string sqlQuery = $"select distinct TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME like '%{ValueToSearchInDbTabs}%'";

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
                        DT = dt.Clone();

                    }
                    else
                    {
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            DT.Rows.Add(item.ItemArray);
                        }
                    }
                }
            }
            DV = DT.DefaultView;
        }

        private void SearchValuesInTablesDb()
        {
            DT = null;
            foreach (var conStr in Connections)
            {
                if (!conStr.IsChecked)
                {
                    continue;
                }

                System.Data.DataTable dt = new System.Data.DataTable();
                string sqlQuery = $"select distinct TABLE_CATALOG, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME like '%{ValueToSearchInDbTabs}%'";

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
                        DT = dt.Clone();

                    }
                    else
                    {
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            DT.Rows.Add(item.ItemArray);
                        }
                    }
                }
            }
            DV = DT.DefaultView;
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
                        DT = dt.Clone();
                        DT.Columns.Add("Повторений", typeof(int));

                    }
                    long resLong;
                    if (long.TryParse(ValueToSearchInDbTabs, out resLong))
                    {
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            System.Data.DataTable dt2 = new System.Data.DataTable();
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
                            }

                        }
                    }
                    else
                    {
                        foreach (System.Data.DataRow item in dt.Rows)
                        {
                            System.Data.DataTable dt2 = new System.Data.DataTable();
                            if (strTypes.Contains(item[3].ToString()) && item[3].ToString() != "varbinary" 
                                && item[3].ToString() != "ntext" && item[3].ToString() != "varchar" 
                                && item[3].ToString() != "smalldatetime" && item[3].ToString() != "timestamp")
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
            DV = DT.DefaultView;
        }
    }
}
