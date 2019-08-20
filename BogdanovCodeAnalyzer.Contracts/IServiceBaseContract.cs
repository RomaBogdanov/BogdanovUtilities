using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BogdanovCodeAnalyzer.Contracts
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServiceBaseCallbackContract))]
    public interface IServiceBaseContract
    {
        /// <summary>
        /// Подключение к серверу. Необходимо, чтобы сервер начал выдавать свои команды по дуплексу
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool Connect();

        /// <summary>
        /// Отключение от сервера. Необходимо, чтобы сервер перестал выдавать команды по дуплексу
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool Disconnect();

        /// <summary>
        /// Передача сообщения с логом
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        /// <param name="method"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [OperationContract]
        bool Log(string message, string tag, string method, string file);
        /*
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
        */
        // TODO: Добавьте здесь операции служб
    }

    [ServiceContract]
    public interface IServiceBaseCallbackContract
    {
        /// <summary>
        /// Начинаем записывать логи
        /// </summary>
        [OperationContract]
        void StartLogs();
        /// <summary>
        /// Прекращаем записывать логи
        /// </summary>
        [OperationContract]
        void StopLogs();
    }

    // Используйте контракт данных, как показано на следующем примере, чтобы добавить сложные типы к сервисным операциям.
    // В проект можно добавлять XSD-файлы. После построения проекта вы можете напрямую использовать в нем определенные типы данных с пространством имен "BogdanovCodeAnalyzer.Contracts.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
