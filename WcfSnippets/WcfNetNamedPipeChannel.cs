using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfSnippets
{
    /// <summary>
    /// Список шаблонных кодов для сниппетов для создания WCF соединения
    /// по протоколу NetNamedPipeBinding
    /// </summary>
    class WcfNetNamedPipeChannel
    {
        /// <summary>
        /// Код с созданием простого WCF сервера
        /// </summary>
        public void CreateSimpleService()
        {
            // ПРИМЕЧАНИЯ:
            // 1. Используется для межпроцессных взаимодействий на одной машине.
            // 1. Два сервера с одним URI запустить нельзя
            // 2. Если откроется несколько клиентов, то каждому будет свой ответ по запросу, 
            // т.е. клиенты будут общаться с сервером раздельно.

            // Чтобы заработало, надо подключить библиотеку System.ServiceModel, а затем пространство имён
            // using System.ServiceModel;

            Uri address = new Uri("net.pipe://localhost/TestPype/IContract"); // A - Address  Адрес
            NetNamedPipeBinding binding = new NetNamedPipeBinding();          // B - Binding  Привязка
            Type contract = typeof(IContract);                                // С - Contract Контракт

            ServiceHost host = new ServiceHost(typeof(Service));              // Создаём объект хоста
            host.AddServiceEndpoint(contract, binding, address);              // Прибавляем конечную точку к хосту
            host.Open();                                                      // Открываем сервер
            Console.WriteLine("Запустили сервер...");
            Console.ReadLine();
            host.Close();                                                     // Закрываем сервер

            // Пример реализации интерфейса и контракта
            // ========================================

            //[ServiceContract]
            //interface IContract
            //{
            //    [OperationContract]
            //    string Say(string input);
            //}

            //class Service : IContract
            //{
            //    public string Say(string input)
            //    {
            //        Console.WriteLine($"Пришло сообщение {input}");
            //        return $"Сообщение {input} получил";
            //    }
            //}

            // ========================================
        }

        public void CreateSimpleClient()
        {
            Uri address = new Uri("net.pipe://localhost/TestPype/IContract"); // A - Address  Адрес
            NetNamedPipeBinding binding = new NetNamedPipeBinding();          // B - Binding  Привязка

            ChannelFactory<IContract> factory = new ChannelFactory<IContract>(binding, address.AbsoluteUri);
            IContract channel = factory.CreateChannel();                      // Канал для прокси.

            // Пример работы
            //while (true)
            //{
            //    Console.Write("Отправить сообщение:");
            //    string message = Console.ReadLine();
            //    Console.WriteLine(channel.Say(message));
            //}

            // Пример реализации интерфейса и контракта
            // ========================================

            //[ServiceContract]
            //interface IContract
            //{
            //    [OperationContract]
            //    string Say(string input);
            //}

            // ========================================
        }
    }

    [ServiceContract]
    interface IContract
    {
        [OperationContract]
        string Say(string input);
    }

    class Service : IContract
    {
        public string Say(string input)
        {
            Console.WriteLine($"Пришло сообщение {input}");
            return $"Сообщение {input} получил";
        }
    }
}
