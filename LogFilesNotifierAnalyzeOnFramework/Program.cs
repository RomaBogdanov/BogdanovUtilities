using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFilesNotifierAnalyzeOnFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            // Проработать функционал сведения всех файлов лога в один и оставить только строки 4 потока.
            //AllLogsToOneWithFilter4Thread();
            ParsingLog();

            Console.WriteLine("Закончил");
            Console.ReadLine();
        }

        static void AllLogsToOneWithFilter4Thread(
            string logsDir = @"\\programserver\Share\Логи нотифаера\16_11_2020\",
            string endLogFile = @"F:\Помойка\FinalLog\")
        {
            var allFiles = System.IO.Directory.GetFiles(logsDir);
            foreach (var item in allFiles)
            {
                var lines = System.IO.File.ReadAllLines(item, Encoding.Default).Where(p => p.Length > 31 && p[31] == '4');
                System.IO.File.AppendAllLines($"{endLogFile}finishLog.txt", lines);
                //Console.WriteLine(lines.ElementAt(0)[31]);
                //foreach (var line in lines)
                //{
                //    System.IO.File.AppendAllLines($"{endLogFile}finishLog.txt",
                //        lines, System.Text.Encoding.Unicode);
                //}
            }
        }

        static void ParsingLog(string logFile = @"F:\Помойка\FinalLog\finishLog.txt")
        {
            var lines = System.IO.File.ReadAllLines(logFile);
            List<LogStringParser> logs = new List<LogStringParser>();
            //DateTime dt = Convert.ToDateTime(lines[0].Substring(0, 19));
            //DateTime dt = DateTime.Parse(lines[0].Substring(0, 19));
            //Console.WriteLine(lines[0].Substring(40));
            //Console.WriteLine(dt);
            foreach (var item in lines)
            {
                logs.Add(ParseLine(item));
            }
            CountOfRepeating(logs);
            Delays(logs);
        }

        static LogStringParser ParseLine(string logLine)
        {
            try
            {
                DateTime d;

                bool result = DateTime.TryParseExact(logLine.Substring(0, 23), "yyyy-MM-dd HH:mm:ss,fff",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out d);

                LogStringParser logStringParser = new LogStringParser
                {
                    Time = d,//Convert.ToDateTime(logLine.Substring(0, 19)),
                    MessageBody = logLine.Substring(40)
                };
                return logStringParser;

            }
            catch (Exception)
            {
                return null;
            }
        }


        private static void CountOfRepeating(List<LogStringParser> logs)
        {
            string path = @"F:\Помойка\FinalLog\RepeatingLogs.txt";
            Dictionary<string, int> repeats = new Dictionary<string, int>();
            foreach (var log in logs)
            {
                if (log != null)
                {
                    if (repeats.ContainsKey(log.MessageBody))
                    {
                        repeats[log.MessageBody]++;
                    }
                    else
                    {
                        repeats.Add(log.MessageBody, 0);
                    }
                }
            }
            var a = repeats.OrderByDescending(p => p.Value).ToList();
            foreach (var item in a)
            {
                System.IO.File.AppendAllText(path, $"{item.Value} \t{item.Key}" + Environment.NewLine);
            }
        }

        private static void Delays(List<LogStringParser> logs)
        {
            string path = @"F:\Помойка\FinalLog\Delays.txt";
            List<DelDetails> dels = new List<DelDetails>();
            var a = logs.Where(p => p != null).OrderBy(p => p.Time).ToList();
            DateTime oldTime = a[0].Time;
            for (int i = 1; i < a.Count; i++)
            {
                if (a[i].Time != a[i - 1].Time)
                {
                    dels.Add(new DelDetails
                    {
                        OldTime = oldTime,
                        NewTime = a[i].Time,
                        Delay = a[i].Time - a[i - 1].Time,
                        BeginEvent = a[i - 1].MessageBody,
                        EndEvent = a[i].MessageBody
                    });
                    oldTime = a[i].Time;
                }
            }
            var b = dels.OrderByDescending(p => p.Delay).Take(300).ToList();
            foreach (var item in b)
            {
                System.IO.File.AppendAllText(path, $"{item.Delay} | {item.OldTime} | {item.NewTime} | {item.BeginEvent}" + Environment.NewLine);
                var c = logs.Where(p => p != null).Where(p => p.Time == item.OldTime).ToList();
                //foreach (var item2 in c)
                //{
                //    System.IO.File.AppendAllText(path, $"{item2.MessageBody}" + Environment.NewLine);
                //}
            }
            //System.IO.File.AppendAllLines(path, a);
            //foreach (var item in a)
            //{
            //    System.IO.File.AppendAllText(path, item + Environment.NewLine);
            //}
        }

    }

    class LogStringParser
    {
        public DateTime Time { get; set; }
        public string MessageBody { get; set; }
    }

    class DelDetails
    {
        public DateTime OldTime { get; set; }
        public DateTime NewTime { get; set; }
        public TimeSpan Delay { get; set; }

        public string BeginEvent { get; set; }
        public string EndEvent { get; set; }
    }
}
