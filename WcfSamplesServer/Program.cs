using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfSamplesServer
{
    class Program : ITest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Виды тестов (выбрать цифру):");
            Console.WriteLine("1. Тестирование NetNamedPipeBinding");
            switch (Console.ReadLine())
            {
                case "1":
                    Description(File.ReadAllText("Descripts\\NetNamedPipeBindingDescript.txt"));
                    NetNamedPipeBindingWithCallbackTest();

                    break;
                default:
                    break;
            }

            Console.ReadLine();

        }

        /// <summary>
        /// Тестирует WCF соединение через NetNamedPipeBinding c возможностью
        /// дуплексной работы.
        /// </summary>
        private static void NetNamedPipeBindingWithCallbackTest()
        {
            Uri baseAddress = new Uri("http://localhost:8015/wcfsamples/netnamedpipe");
            string address = "net.pipe://localhost/wcfsamples/test";
            using (ServiceHost sh = new ServiceHost(typeof(Program), baseAddress))
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                sh.AddServiceEndpoint(typeof(ITest), binding, address);

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri("http://localhost:8016/wcfsamples");
                sh.Description.Behaviors.Add(smb);

                sh.Open();
                Console.WriteLine("Сервер готов к работе");
                Console.ReadLine();
                Console.WriteLine("Сервер закончил работу");
                sh.Close();
            }
        }

        static void Description(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(txt);
            Console.ResetColor();
        }

        public string Meth1(string helloMessage)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(helloMessage);
            Console.ResetColor();
            var callback = OperationContext.Current.GetCallbackChannel<ITestCallback>();
            callback.MethCallBack1("Сообщение по Callback");
            return "Сервак преветствует тебя!";
        }
    }

    /// <summary>
    /// Контракт приёма сообщений.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ITestCallback))]
    public interface ITest
    {
        [OperationContract]
        string Meth1(string helloMessage);
    }

    /// <summary>
    /// Контракт колбэка для обратной связи.
    /// </summary>
    [ServiceContract]
    public interface ITestCallback
    {
        [OperationContract(IsOneWay = true)]
        void MethCallBack1(string msg);
    }
}
