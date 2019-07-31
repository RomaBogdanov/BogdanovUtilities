using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
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
    /// 1. Помечать файлы, которые не надо покрывать логами;
    /// 2. Помечать методы, которые не надо покрывать логами;
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
        private string pathToAnalyzeFiles;
        private string textInTextBlock;
        private string textAddedNamespase;

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

        public string PathToAnalyzeFiles
        {
            get => pathToAnalyzeFiles;
            set
            {
                pathToAnalyzeFiles = value;
                OnPropertyChanged();
            }
        }

        public string TextInTextBlock
        {
            get => textInTextBlock;
            set
            {
                textInTextBlock = value;
                OnPropertyChanged();
            }
        }

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
        /// Команда на запуск сервера.
        /// </summary>
        public ICommand StartStopServerCommand { get; set; }

        /// <summary>
        /// Команда для покрытия кода логами.
        /// </summary>
        public ICommand CreateLogsInCodeCommand { get; set; }

        public MainWindowViewModel()
        {
            AggregatorMessages.OnMessageFromClient += AggregatorMessages_OnMessageFromClient;
            StartStopServerCommand = new RelayCommand(obj => StartStopServer());
            CreateLogsInCodeCommand = new RelayCommand(obj => CreateLogsInCode());
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

    static class AggregatorMessages
    {
        public static event Action<Message> OnMessageFromClient;

        public static void ReceiveMessage(Message message)
        {
            OnMessageFromClient?.Invoke(message);
        }
    }
}
