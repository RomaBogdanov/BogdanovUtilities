using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Description;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace BogdanovCodeAnalyzer.ViewModel
{
    class RemotingProgramsLogsViewModel : NotifyPropertyChanged
    {
        ServiceHost host;
        private string taskToOpenOrCloseServer = "Запустить сервер";

        private bool startRecLogs = false;
        private string logsRec = "Начать запись логов";
        private ObservableCollection<Message> messages = new ObservableCollection<Message>();

        /// <summary>
        /// Команда на запуск сервера.
        /// </summary>
        public ICommand StartStopServerCommand { get; set; }

        /// <summary>
        /// Команда запуска остановки записи логов.
        /// </summary>
        public ICommand StartStopRecLogsCommand { get; set; }

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

        public RemotingProgramsLogsViewModel()
        {

            AggregatorMessages.OnMessageFromClient += AggregatorMessages_OnMessageFromClient;
            StartStopServerCommand = new RelayCommand(obj => StartStopServer());
            StartStopRecLogsCommand = new RelayCommand(obj => StartStopRecLogs());

            
        }

        /// <summary>
        /// Процедура включения/отключения записи логов
        /// </summary>
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
                Uri baseAddress = new Uri("http://localhost:8015/logs/netnamedpipe");
                string address = "net.pipe://localhost/logs/ServiceBaseContract";
                host = new ServiceHost(typeof(ServiceBaseContract), baseAddress);
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                host.AddServiceEndpoint(typeof(BogdanovCodeAnalyzer.Contracts.IServiceBaseContract), binding, address);

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://localhost:8016/logs");
                host.Description.Behaviors.Add(smb);

                host.Open();
                TaskToOpenOrCloseServer = "Закрыть сервер";
            }
            else
            {
                host.Close();
                TaskToOpenOrCloseServer = "Запустить сервер";
            }



            /*if (host == null || host.State != CommunicationState.Opened)
            {
                host = new ServiceHost(typeof(ServiceBaseContract));
                host.Open();
                TaskToOpenOrCloseServer = "Закрыть сервер";
            }
            else
            {
                host.Close();
                TaskToOpenOrCloseServer = "Запустить сервер";
            }*/
        }

        /// <summary>
        /// Добавление сообщения.
        /// </summary>
        /// <param name="message"></param>
        private void AggregatorMessages_OnMessageFromClient(Message message)
        {
            /*if (startRecLogs)
            {*/
                Messages.Add(message);
            //}
        }

    }


    /// <summary>
    /// Контракт для взаимодействия с внешними приложениями.
    /// </summary>
    class ServiceBaseContract : BogdanovCodeAnalyzer.Contracts.IServiceBaseContract
    {
        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

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

}
