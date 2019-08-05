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
using BogdanovUtilitisLib.LogsWrapper;
using r = BogdanovUtilitisLib.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BogdanovCodeAnalyzer.ViewModel
{
    /// <summary>
    /// Обработчик формы анализатора
    /// </summary>
    /// <remarks>
    /// Реализовать следующий функционал по логам:
    /// 1.1. Помечать файлы, которые не надо покрывать логами;
    /// 1.2. Помечать методы, которые не надо покрывать логами;
    /// 1.3. Помечать классы, которые не надо покрывать логами;
    /// 3. Обработка файла логгирования:
    /// 3.1. Получение частотности входа в метод;
    /// 3.2. Получение частотности использования класса;
    /// 3.3. Получение частотности использования файла;
    /// 4. Сохранять результаты настроек по обкладке логами кода;
    /// 5. Сделать логгирование внутри catch;
    /// 6. Вынести процесс обкладывания логами в отдельную задачу;
    /// 7. Сделать авторазворачивание инфраструктуры логгирования в проекте:
    /// 7.1. Подключение необходимых библиотек;
    /// 7.2. Настройка конфигурационных файлов;
    /// 7.3. Настройка проекта;
    /// 8. Сравнивать результаты разных логов:
    /// 8.1. Смотреть сходства в логах;
    /// 8.2. Смотреть различия в логах;
    /// 9. Сохранять состояние по объектам:
    /// 9.1. Научиться находить неизменяемую часть состояний объектов;
    /// 9.2. Научиться находить изменяемую часть состояний объектов;
    /// 10. Научиться удалять обкладку логами без отката системы.
    /// 
    /// Реализация функционала по взаимодействию с БД T-SQL:
    /// 1. Уметь находить все таблицы с колонками в базе данных содержащие поле с данными;
    /// 1.1. Тоже самое, но с массивом полей данных;
    /// 2. Уметь находить все таблицы с колонками в массиве баз данных содержащие поле с данными;
    /// 2.1. Тоже самое, но с массивом полей данных.
    /// 3. Исключать таблицы из рассмотрения.
    /// </remarks>
    class MainWindowViewModel : NotifyPropertyChanged
    {
        ServiceHost host;
        private string taskToOpenOrCloseServer = "Запустить сервер";
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private string pathToAnalyzeFiles;
        private string textInTextBlock;
        private string textAddedNamespase;
        private string logFilesPath = @"C:\BogdanovR\Experiments\Sintez\SintezOSPClient\bin\Debug\Logs\Log.log";
        private bool isFrequenceRepeatStrings;
        private bool isFrequenceRepeatFiles;
        private ObservableCollection<Log> logs;
        private string connectionToDBStr = "Data Source=localhost;Initial Catalog=gorizont;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True";
        private string valueToSearchInDbTabs;
        private System.Data.DataTable dT;
        private ObservableCollection<ConnectionDB> connections;
        private System.Data.DataView dV;

        /// <summary>
        /// Сообщения, получаемые от клиентов.
        /// </summary>
        public ObservableCollection<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Информация по логам.
        /// </summary>
        public ObservableCollection<Log> Logs
        {
            get => logs;
            set
            {
                logs = value;
                OnPropertyChanged();
            }
        }

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
        /// Показывает статус задачи открытия или закрытия сервера.
        /// </summary>
        public string TaskToOpenOrCloseServer
        {
            get => taskToOpenOrCloseServer;
            set
            {
                taskToOpenOrCloseServer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к файлам, которые будут покрываться логами.
        /// </summary>
        public string PathToAnalyzeFiles
        {
            get => pathToAnalyzeFiles;
            set
            {
                pathToAnalyzeFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текст выводимый в текстовый блок.
        /// </summary>
        public string TextInTextBlock
        {
            get => textInTextBlock;
            set
            {
                textInTextBlock = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование добавляемого пространства имён.
        /// </summary>
        public string TextAddedNamespase
        {
            get => textAddedNamespase;
            set
            {
                textAddedNamespase = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к анализируемому лог-файлу.
        /// </summary>
        public string LogFilesPath
        {
            get => logFilesPath;
            set
            {
                logFilesPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Флаг того, что ищется частота встречаемости строки в логах
        /// </summary>
        public bool IsFrequenceRepeatStrings
        {
            get => isFrequenceRepeatStrings;
            set
            {
                isFrequenceRepeatStrings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Флаг того, что ищется частота встречаемости файлов в логах
        /// </summary>
        public bool IsFrequenceRepeatFiles
        {
            get => isFrequenceRepeatFiles;
            set
            {
                isFrequenceRepeatFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Строка подключения к Базе данных.
        /// </summary>
        public string ConnectionToDBStr
        {
            get => connectionToDBStr;
            set
            {
                connectionToDBStr = value;
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
        /// Команда на запуск сервера.
        /// </summary>
        public ICommand StartStopServerCommand { get; set; }

        /// <summary>
        /// Команда для покрытия кода логами.
        /// </summary>
        public ICommand CreateLogsInCodeCommand { get; set; }

        /// <summary>
        /// Команда исследования логов
        /// </summary>
        public ICommand SearchFrequencyLogsCommand { get; set; }

        /// <summary>
        /// Команда для поиска таблиц и столбцов в БД для поиска полей.
        /// </summary>
        public ICommand SearchValuesInFieldsDbCommand { get; set; }

        public MainWindowViewModel()
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
            /*
            Connections.Add(new ConnectionDB
            {
                IsChecked = true,
                ConnectionString = "Data Source=localhost;Initial Catalog=gorizont;Persist Security Info=True;User ID=profcert;Password=12345;MultipleActiveResultSets=True"
            });*/

            AggregatorMessages.OnMessageFromClient += AggregatorMessages_OnMessageFromClient;
            StartStopServerCommand = new RelayCommand(obj => StartStopServer());
            CreateLogsInCodeCommand = new RelayCommand(obj => CreateLogsInCode());
            SearchFrequencyLogsCommand = new RelayCommand(obj => SearchFrequencyLogs());
            SearchValuesInFieldsDbCommand = new RelayCommand(obj => SearchValuesInFieldsDb());
        }

        /// <summary>
        /// Ищет таблицы и колонки в БД содержащие значение с полем.
        /// </summary>
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

        /// <summary>
        /// Процедура запуска сервера.
        /// </summary>
        private void StartStopServer()
        {
            if (host == null || host.State != CommunicationState.Opened)
            {
                host = new ServiceHost(typeof(ServiceBaseContract));
                host.Open();
                TaskToOpenOrCloseServer = "Закрыть сервер";
            }
            else
            {
                host.Close();
                TaskToOpenOrCloseServer = "Запустить сервер";
            }
        }

        /// <summary>
        /// Покрытие кода логами.
        /// </summary>
        /// <remarks>
        /// Обязательно смотри описание к классу.
        /// </remarks>
        private void CreateLogsInCode()
        {
            r.CodeGenerator codeGenerator = new r.CodeGenerator();
            r.CodeAnalyzer codeAnalyzer;

            IEnumerable<string> paths = System.IO.Directory.EnumerateFiles(PathToAnalyzeFiles, "*.cs",
                System.IO.SearchOption.AllDirectories);
            paths.Where(p => !p.ToUpper().Contains("DESIGNER.CS"));
            TextInTextBlock = "";
            foreach (var item in paths.Where(p => !p.ToUpper().Contains("DESIGNER.CS")))
            {
                //TextInTextBlock += item + Environment.NewLine;
                codeAnalyzer = new r.CodeAnalyzer(item);
                var root = codeAnalyzer.SyntaxTree.GetRoot();
                var methods = codeAnalyzer.SearchMethods();
                var expression1 = codeGenerator.CreatingCallProcedureExpression(
                    "Logger.Debug", new List<string> { "\"Начало метода\"" });
                var expression2 = codeGenerator.CreatingCallProcedureExpression(
                    "Logger.Debug", new List<string> { "\"Окончание метода\"" });

                Func<SyntaxNode, SyntaxNode, SyntaxNode> func = (x, y) =>
                {
                    y = codeGenerator.AddExpressionToStartMethodsBody(
                       x as MethodDeclarationSyntax, expression1);
                    y = codeGenerator.AddExpressionToFinishOrBeforeReturnMethodsBody(
                        y as MethodDeclarationSyntax, expression2);
                    return y;
                };
                root = root.ReplaceNodes(methods, func).NormalizeWhitespace();

                // вставляем пространство имён
                string nameNamespace = TextAddedNamespase;
                UsingDirectiveSyntax usdir = codeGenerator.CreatingUsingDirective(nameNamespace);
                List<SyntaxNode> usdirs = codeAnalyzer.SearchLinkedNamespaces(root);

                if (usdirs.FirstOrDefault(p => ((p as UsingDirectiveSyntax).Name
                    .GetText().ToString() == nameNamespace)) == null && usdirs.Count > 0)
                {
                    root = root.InsertNodesAfter(usdirs[usdirs.Count - 1],
                        new List<SyntaxNode> { usdir }).NormalizeWhitespace();
                }
                System.IO.File.WriteAllText(item, root.GetText().ToString(), Encoding.UTF8);
            }
            TextInTextBlock = "Добавление логов закончено";
        }

        /// <summary>
        /// Анализ лога.
        /// </summary>
        private void SearchFrequencyLogs()
        {
            if (IsFrequenceRepeatStrings)
            {
                if (Logs != null)
                {
                    Logs.Clear();
                }
                else
                {
                    Logs = new ObservableCollection<Log>();
                }
                var logs = System.IO.File.ReadLines(LogFilesPath, Encoding.Default);
                //var aa = System.IO.File.ReadAllText(LogFilesPath, Encoding.Default);
                string str = "";
                Dictionary<string, long> dict = new Dictionary<string, long>();
                foreach (var item in logs)
                {
                    str = item.Substring(25);
                    if (dict.Keys.Contains(str))
                    {
                        dict[str]++;
                    }
                    else
                    {
                        dict.Add(str, 1);
                    }
                }
                var a = from l in dict.OrderByDescending(p => p.Value)
                        select new Log { MsgLog = l.Key, Count = l.Value };
                Logs = new ObservableCollection<Log>(a);
            }
            if (IsFrequenceRepeatFiles)
            {
                if (Logs != null)
                {
                    Logs.Clear();
                }
                else
                {
                    Logs = new ObservableCollection<Log>();
                }
                var logs = System.IO.File.ReadLines(LogFilesPath, Encoding.Default);
                string str = "";
                Dictionary<string, long> dict = new Dictionary<string, long>();
                foreach (var item in logs)
                {
                    str = Regex.Match(item, @"C:\\.*").Value;
                    if (dict.Keys.Contains(str))
                    {
                        dict[str]++;
                    }
                    else
                    {
                        dict.Add(str, 1);
                    }
                }
                var a = from l in dict.OrderByDescending(p => p.Value)
                        select new Log { MsgLog = l.Key, Count = l.Value };
                Logs = new ObservableCollection<Log>(a);
            }
        }

        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="message"></param>
        private void AggregatorMessages_OnMessageFromClient(Message message)
        {
            Messages.Add(message);
        }

    }

    /// <summary>
    /// Контракт для взаимодействия с внешними приложениями.
    /// </summary>
    class ServiceBaseContract : BogdanovCodeAnalyzer.Contracts.IServiceBaseContract
    {
        public bool Log(string message, string tag, string method, string file)
        {
            AggregatorMessages.ReceiveMessage(new Message
            {
                LogMessage = message,
                Tag = tag,
                Method = method,
                File = file
            });

            return true;
        }
    }

    /// <summary>
    /// Сообщение от клиента.
    /// </summary>
    class Message
    {
        public string LogMessage { get; set; }
        public string Tag { get; set; }
        public string Method { get; set; }
        public string File { get; set; }
    }

    /// <summary>
    /// Информация по логу.
    /// </summary>
    class Log
    {
        public string MsgLog { get; set; }
        public long Count { get; set; }
    }

    class ConnectionDB
    {
        /// <summary>
        /// Выбрано ли подключение для исследования?
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString { get; set; }
    }

    static class AggregatorMessages
    {
        public static event Action<Message> OnMessageFromClient;

        public static void ReceiveMessage(Message message)
        {
            OnMessageFromClient?.Invoke(message);
        }
    }
}
