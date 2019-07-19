using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogdanovUtilsLib.MSILHandle.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Reflection.MethodInfo mi = typeof(A).GetMethod("Meth1");
            Console.WriteLine(BogdanovUtilitisLib.MSILHandle.MSILHandler.Disassemble(mi));
            Console.ReadLine();

        }
    }

    class A
    {
        public int Meth1()
        {
            BogdanovUtilitisLib.LogsWrapper.Logger.Debug("Вход в метод");
            int i = 10;
            int j = 12;
            int k = i * j;
            BogdanovUtilitisLib.LogsWrapper.Logger.Debug("Выход из метода");
            return k;
        }
    }
}
