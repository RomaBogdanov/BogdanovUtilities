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

namespace BogdanovCodeAnalyzer.ViewModel
{
    class MainWindowViewModel : NotifyPropertyChanged
    {
        ServiceHost host;
        private string taskToOpenOrCloseServer = "Запустить сервер";
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();

        public ObservableCollection<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }

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
        /// Команда на запуск сервера.
        /// </summary>
        public ICommand StartStopServerCommand { get; set; }

        public MainWindowViewModel()
        {
            AggregatorMessages.OnMessageFromClient += AggregatorMessages_OnMessageFromClient;
            StartStopServerCommand = new RelayCommand(obj => StartStopServer());
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
