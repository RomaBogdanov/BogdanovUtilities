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
    /// </remarks>
    class MainWindowViewModel : NotifyPropertyChanged
    {
        ServiceHost host;
        private string taskToOpenOrCloseServer = "Запустить сервер";
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();
        private string pathToAnalyzeFiles = @"C:\BogdanovR\Experiments\Sintez\";
        private string textInTextBlock;
        private string textAddedNamespase = "SintezLibrary";
        private string logFilesPath = @"C:\BogdanovR\Experiments\Sintez\SintezOSPClient\bin\Debug\Logs\Log.log";
        private bool isFrequenceRepeatStrings;
        private bool isFrequenceRepeatFiles;
        private ObservableCollection<Log> logs;

        private bool startRecLogs = false;
        private string logsRec= "Начать запись логов";

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
        /// Сообщение о записи логов
        /// </summary>
        public string LogsRec
        {
            get => logsRec;
            set
            {
                logsRec = value;
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
        /// Команда на запуск сервера.
        /// </summary>
        public ICommand StartStopServerCommand { get; set; }

        /// <summary>
        /// Команда запуска остановки записи логов.
        /// </summary>
        public ICommand StartStopRecLogsCommand { get; set; }

        /// <summary>
        /// Команда для покрытия кода логами.
        /// </summary>
        //public ICommand CreateLogsInCodeCommand { get; set; }

        /// <summary>
        /// Команда исследования логов
        /// </summary>
        public ICommand SearchFrequencyLogsCommand { get; set; }

        public MainWindowViewModel()
        {
            AggregatorMessages.OnMessageFromClient += AggregatorMessages_OnMessageFromClient;
            StartStopServerCommand = new RelayCommand(obj => StartStopServer());
            //CreateLogsInCodeCommand = new RelayCommand(obj => CreateLogsInCode());
            SearchFrequencyLogsCommand = new RelayCommand(obj => SearchFrequencyLogs());
            StartStopRecLogsCommand = new RelayCommand(obj => StartStopRecLogs());
        }

        private void StartStopRecLogs()
        {
            startRecLogs = !startRecLogs;
            if (startRecLogs) { LogsRec = "Остановить запись логов"; }
            else { LogsRec = "Начать запись логов"; }
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
        /*private void CreateLogsInCode()
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

                // вставляем логи в начало и конец методов
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
                root = root.ReplaceNodes(methods, func);

                // вставляем логи в catch
                var catchs = codeAnalyzer.SearchCatches(root);
                Func<SyntaxNode, SyntaxNode, SyntaxNode> func1 = (x, y) =>
                {
                    y = codeGenerator.AddExpressionToCatchConstructionMethodsBody(
                        x as CatchClauseSyntax, "Logger.Error", "err", "Exception");
                    return y;
                };

                root = root.ReplaceNodes(catchs, func1);


                // вставляем пространство имён
                string nameNamespace = TextAddedNamespase;
                UsingDirectiveSyntax usdir = codeGenerator.CreatingUsingDirective(nameNamespace);
                List<SyntaxNode> usdirs = codeAnalyzer.SearchLinkedNamespaces(root);

                if (usdirs.FirstOrDefault(p => ((p as UsingDirectiveSyntax).Name
                    .GetText().ToString() == nameNamespace)) == null && usdirs.Count > 0)
                {
                    root = root.InsertNodesAfter(usdirs[usdirs.Count - 1],
                        new List<SyntaxNode> { usdir });
                }
                System.IO.File.WriteAllText(item, root.NormalizeWhitespace().GetText().ToString(), Encoding.UTF8);
            }
            TextInTextBlock = "Добавление логов закончено";
        }*/

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
            if (startRecLogs)
            {
                Messages.Add(message);
            }
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

    /// <summary>
    /// Обёртка для подключения к Базе данных
    /// </summary>
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

    /// <summary>
    /// Статический класс для приёма сообщений от клиентов.
    /// </summary>
    static class AggregatorMessages
    {
        public static event Action<Message> OnMessageFromClient;

        public static void ReceiveMessage(Message message)
        {
            OnMessageFromClient?.Invoke(message);
        }
    }
}
