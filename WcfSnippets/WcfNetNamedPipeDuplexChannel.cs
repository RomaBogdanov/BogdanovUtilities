using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfSnippets
{

    /// <summary>
    /// Дуплексное соединение
    /// </summary>
    /// <remarks>
    /// DuplexConnection duplexConnection = new DuplexConnection();
    /// Console.Write("Введите имя сервера: ");
    /// string hostName = Console.ReadLine();
    /// duplexConnection.CreateHost(hostName);
    /// Console.Write("Введите имя сервера для отправки сообщений: ");
    /// string remoteHostName = Console.ReadLine();
    /// while (true)
    /// {
    ///     duplexConnection.SendMessage(remoteHostName, Console.ReadLine());
    /// }
    /// </remarks>
    class DuplexConnection
    {
        /// <summary>
        /// Процедура создания сервера данного приложения.
        /// </summary>
        /// <param name="myPipeName">Именование канала.</param>
        public void CreateHost(string myPipeName)
        {
            // ПРИМЕЧАНИЯ:
            // 1. Используется для межпроцессных взаимодействий на одной машине.
            // 1. Два сервера с одним URI запустить нельзя
            // 2. Если откроется несколько клиентов, то каждому будет свой ответ по запросу, 
            // т.е. клиенты будут общаться с сервером раздельно.

            // Чтобы заработало, надо подключить библиотеку System.ServiceModel, а затем пространство имён
            // using System.ServiceModel;

            Uri HostUri = new Uri($"net.pipe://localhost/{myPipeName}/IContract");
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            Type contract = typeof(IContractDuplex);

            ServiceHost host = new ServiceHost(typeof(ServiceDuplex));
            host.AddServiceEndpoint(contract, binding, HostUri);
            host.Open();
            Console.WriteLine("Запустили сервер...");
        }

        public void SendMessage(string channelName, string message)
        {
            Uri address = new Uri($"net.pipe://localhost/{channelName}/IContract"); // A - Address  Адрес
            NetNamedPipeBinding binding = new NetNamedPipeBinding();          // B - Binding  Привязка

            ChannelFactory<IContractDuplex> factory = new ChannelFactory<IContractDuplex>(binding, address.AbsoluteUri);
            IContractDuplex channel = factory.CreateChannel();                      // Канал для прокси.
            try
            {
                channel.SendMessage(message);
            }
            catch (EndpointNotFoundException err)
            {
                Console.WriteLine(err.Message);
            }
            catch (FaultException err)
            {
                // Любопытно, что исключение не роняет сервер, но может уронить клиента.
                Console.WriteLine(err.Message);
            }
        }
    }

    [ServiceContract]
    interface IContractDuplex
    {
        [OperationContract]
        void SendMessage(string message);
    }

    class ServiceDuplex : IContractDuplex
    {
        public void SendMessage(string message)
        {
            Console.WriteLine($"Пришло сообщение: {message}");
            // Генерация на клиенте FaultException 
            // throw new Exception("Fuck!!!");
        }
    }
}
