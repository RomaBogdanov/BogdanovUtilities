using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Globalization;
using BogdanovUtilitisLib.MVVMUtilsWrapper;

namespace BogdanovCodeAnalyzer.ViewModel
{
    /// <summary>
    /// Анализ файла логгирования.
    /// </summary>
    /// <remarks>
    /// 1. Обработка файла логгирования:
    /// v------ 1.1. Получение частотности входа в метод;
    /// 1.2. Получение частотности использования класса;
    /// v------ 1.3. Получение частотности использования файла;
    /// 2. Сделать фильтрацию:
    /// 2.1. По логам в методе;
    /// 2.2. По логам в классе;
    /// 2.3. По логам в файле;
    /// </remarks>
    class AnalyzeLogsViewModel : NotifyPropertyChanged
    {
        private string pathToAnalyzeFiles = @"C:\BogdanovR\Experiments\Sintez\";
        private string textInTextBlock;
        private string textAddedNamespase = "SintezLibrary";
        //private string logFilesPath = @"C:\BogdanovR\Experiments\Sintez\SintezOSPClient\bin\Debug\Logs\Log.log";
        //private string logFilesPath = @"C:\BogdanovR\MyReps\BogdanovUtilities\BogdanovCodeAnalyzer\bin\Debug\Logs\Log.log";
        private string logFilesPath = @"\\programserver\Share\Logs\Log.log.1";
        private bool isFrequenceRepeatStrings;
        private bool isFrequenceRepeatFiles;
        private ObservableCollection<Log> logs;



        /// <summary>
        /// Информация по логам.
        /// </summary>
        public ObservableCollection<Log> Logs
        {
            get => logs;
            set
            {
                logs = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к файлам, которые будут покрываться логами.
        /// </summary>
        public string PathToAnalyzeFiles
        {
            get => pathToAnalyzeFiles;
            set
            {
                pathToAnalyzeFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текст выводимый в текстовый блок.
        /// </summary>
        public string TextInTextBlock
        {
            get => textInTextBlock;
            set
            {
                textInTextBlock = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование добавляемого пространства имён.
        /// </summary>
        public string TextAddedNamespase
        {
            get => textAddedNamespase;
            set
            {
                textAddedNamespase = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Путь к анализируемому лог-файлу.
        /// </summary>
        public string LogFilesPath
        {
            get => logFilesPath;
            set
            {
                logFilesPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Флаг того, что ищется частота встречаемости строки в логах
        /// </summary>
        public bool IsFrequenceRepeatStrings
        {
            get => isFrequenceRepeatStrings;
            set
            {
                isFrequenceRepeatStrings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Флаг того, что ищется частота встречаемости файлов в логах
        /// </summary>
        public bool IsFrequenceRepeatFiles
        {
            get => isFrequenceRepeatFiles;
            set
            {
                isFrequenceRepeatFiles = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Команда исследования логов
        /// </summary>
        public ICommand SearchFrequencyLogsCommand { get; set; }

        public AnalyzeLogsViewModel()
        {
            SearchFrequencyLogsCommand = new RelayCommand(obj => SearchFrequencyLogs());
        }

        class Supp
        {
            public TimeSpan DiffTime { get; set; }
            public string Comment { get; set; }
        }

        /// <summary>
        /// Анализ лога.
        /// </summary>
        private void SearchFrequencyLogs()
        {

            if (Logs != null)
            {
                Logs.Clear();
            }
            else
            {
                Logs = new ObservableCollection<Log>();
            }
            var logs = System.IO.File.ReadLines(LogFilesPath, Encoding.Default);
            string str = "";
            // =================
            Dictionary<string, Supp> dsd = new Dictionary<string, Supp>();
            string[] cultureNames = { "en-US", "ru-RU", "ja-JP" };
            CultureInfo culture = new CultureInfo("ru-RU");

            //DateTime dt = new DateTime()
            //var a = culture.GetFormat()
            //ICustomFormatter
            DateTime old = new DateTime();
            string oldComment = "";
            bool start = true;
            TimeSpan ddd = new TimeSpan(0, 0, 30);
            foreach (var item in logs)
            {
                if (item.Length < 25) continue;
                str = item.Substring(0, 25);
                if (dsd.Keys.Contains(str)) continue;
                try
                {
                    int year = Convert.ToInt32(str.Substring(0, 4));
                    int month = Convert.ToInt32(str.Substring(5, 2));
                    int day = Convert.ToInt32(str.Substring(8, 2));
                    int hour = Convert.ToInt32(str.Substring(11, 2));
                    int min = Convert.ToInt32(str.Substring(14, 2));
                    int sec = Convert.ToInt32(str.Substring(17, 2));
                    int milisec = Convert.ToInt32(str.Substring(20, 3));
                    DateTime dt = new DateTime(year, month, day, hour, min, sec, milisec);
                    if (start)
                    {
                        old = dt;
                        oldComment = "";
                        start = false;
                    }
                    //TimeSpan timeSpan = dt - old;
                    dsd.Add(str, new Supp { DiffTime = dt - old, Comment = oldComment });
                    old = dt;
                    oldComment = item.Substring(25);
                }
                catch
                {
                    continue;
                }
            }
            var b = from t in dsd.Where(d => d.Value.DiffTime > ddd)//.OrderByDescending(p => p.Value.DiffTime)
                    select new Log { MsgLog = t.Key, Count = t.Value.DiffTime, Comment = t.Value.Comment };
            //var f = b.ToList();
            Logs = new ObservableCollection<Log>(b);
            return;
            // =================
            Dictionary<string, long> dict = new Dictionary<string, long>();

            if (IsFrequenceRepeatStrings)
            {
                foreach (var item in logs)
                {
                    if (item.Length < 25) continue;
                    str = item.Substring(25);
                    if (dict.Keys.Contains(str))
                    {
                        dict[str]++;
                    }
                    else
                    {
                        dict.Add(str, 1);
                    }
                }
                var a = from l in dict.OrderByDescending(p => p.Value)
                        select new Log { MsgLog = l.Key, Count = l.Value };
                Logs = new ObservableCollection<Log>(a);
            }
            if (IsFrequenceRepeatFiles)
            {
                foreach (var item in logs)
                {
                    str = Regex.Match(item, @"C:\\.*").Value;
                    if (dict.Keys.Contains(str))
                    {
                        dict[str]++;
                    }
                    else
                    {
                        dict.Add(str, 1);
                    }
                }
                var a = from l in dict.OrderByDescending(p => p.Value)
                        select new Log { MsgLog = l.Key, Count = l.Value };
                Logs = new ObservableCollection<Log>(a);
            }

        }
    }
}
