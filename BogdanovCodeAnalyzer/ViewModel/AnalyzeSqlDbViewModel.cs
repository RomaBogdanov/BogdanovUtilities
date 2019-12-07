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
using System.IO;

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
    /// 3. Исключать из рассмотрения:
    /// 3.1. Колонки;
    /// 3.2. Таблицы.
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
        private bool searchIsEnabled = true;
        private bool searchAsString;
        private ObservableCollection<Except> exceptions;
        private ObservableCollection<Column> exceptColumns;
        private Except currentException;

        private string exceptDirectory = "ExceptCols";
        private string currentExceptFile = "ExceptColumns.xml";
        System.Xml.Serialization.XmlSerializer columnsSerializer;

        public ObservableCollection<ConnectionDB> Connections
        {
            get => connections;
            set
            {
                connections = value;
                OnPropertyChanged();
            }
        }

        public Except CurrentException
        {
            get => currentException;
            set
            {
                currentException = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Except> Exceptions
        {
            get => exceptions;
            set
            {
                exceptions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Column> ExceptColumns
        {
            get => exceptColumns;
            set
            {
                exceptColumns = value;
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
        /// Статус активности кнопок поиска
        /// </summary>
        public bool SearchIsEnabled
        {
            get => searchIsEnabled;
            set
            {
                searchIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool SearchAsString
        {
            get => searchAsString;
            set
            {
                searchAsString = value;
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

        /// <summary>
        /// Команда для исключения колонки из поиска.
        /// </summary>
        public ICommand ToExceptColsCommand { get; set; }

        /// <summary>
        /// Команда для исключения всех колонок из поиска.
        /// </summary>
        public ICommand AllToExceptColsCommand { get; set; }

        /// <summary>
        /// Команда сохранения исключённых из поиска колонок.
        /// </summary>
        public ICommand SaveExceptColsCommand { get; set; }

        public AnalyzeSqlDbViewModel()
        {
            columnsSerializer = new System.Xml.Serialization.XmlSerializer(
                typeof(ObservableCollection<Column>));
            Connections = new ObservableCollection<ConnectionDB>();
            Exceptions = new ObservableCollection<Except>();
            if (Directory.Exists(exceptDirectory) && 
                File.Exists($"{exceptDirectory}/{currentExceptFile}"))
            {
                var file = File.OpenRead($"{exceptDirectory}/{currentExceptFile}");

                ExceptColumns = (ObservableCollection<Column>)columnsSerializer.Deserialize(file);
            }
            else
            {
                ExceptColumns = new ObservableCollection<Column>();
            }
            RegisterConnects();
            SearchValuesInFieldsDbCommand = new RelayCommand(obj => SearchValuesInFieldsDb());
            SearchValuesInTablesDbCommand = new RelayCommand(obj => SearchValuesInTablesDb());
            SearchValuesInColumnsDbCommand = new RelayCommand(obj => SearchValuesInColumnsDb());
            ToExceptColsCommand = new RelayCommand(obj => ToExceptCols());
            AllToExceptColsCommand = new RelayCommand(obj => AllToExceptCols());
            SaveExceptColsCommand = new RelayCommand(obj => SaveExceptCols());
        }

        /// <summary>
        /// Процедура сохранения списка колонок исключённых из поиска.
        /// </summary>
        private void SaveExceptCols()
        {
            /*List<Column> col = new List<Column>
            {
                new Column{ ConnectionString="sdf", ColName="sdfasaa", TabName="asdfasf"}
            };*/
            /*System.Xml.Serialization.XmlSerializer xmlSerializer = 
                new System.Xml.Serialization.XmlSerializer(typeof(List<Column>));*/
            Directory.CreateDirectory(exceptDirectory);
            FileStream file = File.Create($"{exceptDirectory}/{currentExceptFile}");
            columnsSerializer.Serialize(file, exceptColumns);
            file.Close();
        }

        /// <summary>
        /// Добавление в список исключений
        /// </summary>
        private void ToExceptCols()
        {
            if (!ExceptColumns.Any(p => p.ColName == CurrentException.ColName &&
                p.ConnectionString == CurrentException.ConnectionString &&
                p.TabName == CurrentException.TabName))
            {
                ExceptColumns.Add(new Column
                {
                    ConnectionString = CurrentException.ConnectionString,
                    TabName = CurrentException.TabName,
                    ColName = CurrentException.ColName
                });
            }
        }

        /// <summary>
        /// Добавление всего в список исключений.
        /// </summary>
        private void AllToExceptCols()
        {
            foreach (var item in Exceptions)
            {
                if (!ExceptColumns.Any(p => p.ColName == item.ColName &&
     p.ConnectionString == item.ConnectionString &&
     p.TabName == item.TabName))
                {
                    ExceptColumns.Add(new Column
                    {
                        ConnectionString = item.ConnectionString,
                        TabName = item.TabName,
                        ColName = item.ColName
                    });
                }
            }
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
            Task.Run(() =>
            {
                SearchIsEnabled = false;
                List<string> strTypes = new List<string> { "char", "nchar", 
                    "nvarchar", "varchar", "timestamp", "ntext", "varbinary", 
                    "smalldatetime", "time", "uniqueidentifier" };
                DT = null;
                foreach (var conStr in Connections)
                {
                    var exCols = ExceptColumns.Where(p => p.ConnectionString == conStr.ConnectionString);
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
                        foreach (var exCol in exCols)
                        {
                            var cols = dt.Select($"TABLE_NAME = '{exCol.TabName}' " +
                                $"and COLUMN_NAME = '{exCol.ColName}'");
                            foreach (var row in cols)
                            {
                                dt.Rows.Remove(row);
                            }
                        }
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
                        if (long.TryParse(ValueToSearchInDbTabs, out resLong) && !SearchAsString)
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
                                try
                                {
                                    da.Fill(dt2);
                                }
                                catch (Exception err)
                                {
                                    App.Current.Dispatcher.Invoke(() =>
                                    Exceptions.Add(new Except
                                    {
                                        Message = err.Message,
                                        TabName = item.ItemArray.ElementAt(1).ToString(),
                                        ColName = item.ItemArray.ElementAt(2).ToString(),
                                        ConnectionString = conStr.ConnectionString
                                    }));
                                    // TODO: Сделать здесь потом вывод сообщения об ошибке
                                    continue;
                                }
                                int c = dt2.Rows.Count;
                                if (c > 0)
                                {

                                    DT.Rows.Add(item.ItemArray.ElementAt(0),
                                        item.ItemArray.ElementAt(1),
                                        item.ItemArray.ElementAt(2),
                                        item.ItemArray.ElementAt(3), c);
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
                                    try
                                    {
                                        da.Fill(dt2);
                                    }
                                    catch (Exception err)
                                    {
                                        App.Current.Dispatcher.Invoke(() =>
                                        Exceptions.Add(new Except
                                        {
                                            Message = err.Message,
                                            TabName = item.ItemArray.ElementAt(1).ToString(),
                                            ColName = item.ItemArray.ElementAt(2).ToString(),
                                            ConnectionString = conStr.ConnectionString
                                        }));
                                        continue;
                                    }
                                    int c = dt2.Rows.Count;
                                    if (c > 0)
                                    {
                                        DT.Rows.Add(item.ItemArray.ElementAt(0),
                                            item.ItemArray.ElementAt(1),
                                            item.ItemArray.ElementAt(2),
                                            item.ItemArray.ElementAt(3), c);
                                    }
                                }
                            }
                        }
                    }
                }
                DV = DT.DefaultView;
                SearchIsEnabled = true;
            });
        }

        void RegisterConnects()
        {
            string dbs = "ReportServer,ReportServerTempDB,law,arm,Analytics_copy," +
                "ElectroluxReportsScan,otchet_gtd,DCL,altasvh,altasvh_old,svh," +
                "gtd2012_ED,DCL_new,gtd2011,distribution,altasvh_old_PRO," +
                "gtd2019,ito,gtd2012_NoBlob,ED4gtd,bosco,docs,decl,ALMlog,test," +
                "gtd518,ED3gtd,ED2gtd,Notification,Scheduler,ClientServer,Attorney," +
                "InvoiceConverter,gtdKV,Duties,DispatcherScan,SiemensCMR," +
                "VBSiteClientsSettings,NotificationClients,OrdersMonitoring," +
                "ExpressCargo,ReceptionScan,law_archive,As2Messages,Analytics_old," +
                "ElectroluxOrders,KyoceraOrders,BoschOrders,Examination," +
                "ScannedDeclarations,KonicaOrders,PriceInfo,X5Orders_copy," +
                "VBSiteClientsSettings_test,X5Orders,RobertBosch";
            var d = dbs.Split(',');
            foreach (var item in d)
            {
                Connections.Add(new ConnectionDB
                {
                    IsChecked = true,
                    ConnectionString = $"Data Source=KVTDECLSQL2;Initial Catalog={item};User=dbadmin;Password=nhbnjgjkz"
                });
            }

            /*Connections.Add(new ConnectionDB
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
            });*/
            /*Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=KVTDECLSQL2;Initial Catalog=Notification;User=dbadmin;Password=nhbnjgjkz"
            });
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=KVTDECLSQL2;Initial Catalog=VBSiteClientsSettings;User=dbadmin;Password=nhbnjgjkz"
            });*/

        }
    }
}
