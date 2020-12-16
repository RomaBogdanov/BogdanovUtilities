using System;
using System.Linq;

namespace LogFilesNotifierAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            // Проработать функционал сведения всех файлов лога в один и оставить только строки 4 потока.
            AllLogsToOneWithFilter4Thread();


            Console.WriteLine("Hello World!");
        }

        static void AllLogsToOneWithFilter4Thread(
            string logsDir = @"\\programserver\Share\Логи нотифаера\16_11_2020\",
            string endLogFile = @"F:\Помойка\FinalLog\")
        {
            var allFiles = System.IO.Directory.GetFiles(logsDir);
            foreach (var item in allFiles)
            {
                var lines = System.IO.File.ReadAllLines(item, System.Text.Encoding.ASCII);//.Where(p => p.Length > 31 && p[31] == '4');
                Console.WriteLine(lines.ElementAt(1));
                //foreach (var line in lines)
                {
                    //System.IO.File.AppendAllLines($"{endLogFile}finishLog.txt",
                    //    lines, System.Text.Encoding.Unicode);
                }
            }
        }
    }
}
