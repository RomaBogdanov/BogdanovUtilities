using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfSamplesClient.TestServiceReference;

namespace WcfSamplesClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WcfSamplesClient.TestServiceReference.ITest test = new WcfSamplesClient.TestServiceReference.TestClient(new System.ServiceModel.InstanceContext(new TestCallback()));
            Console.WriteLine(test.Meth1("Привет!!!"));
            
            Console.ReadLine();
        }
    }

    class TestCallback : WcfSamplesClient.TestServiceReference.ITestCallback
    {
        public void MethCallBack1(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
