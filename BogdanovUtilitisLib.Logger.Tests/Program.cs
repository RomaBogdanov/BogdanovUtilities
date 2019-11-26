using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BogdanovUtilitisLib.LogsWrapper;
using BogdanovUtilitisLib.Logger.Tests.Cases;

namespace BogdanovUtilitisLib.Logger.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            LogsWrapper.Logger.InitLogger();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Введите:");
                Console.WriteLine("0 - выйти из программы");
                Console.WriteLine("1 - Простая проверка выброса");
                Console.WriteLine("2 - Сложная проверка выброса с заходом в другие методы");

                string input = Console.ReadLine();

                TestClasses1 test = new TestClasses1();
                switch (input)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        test.AnalyzeLog(true);
                        break;
                    case "2":
                        test.AnalyzeLog2(true);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
