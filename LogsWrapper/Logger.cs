using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Config;
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;*/

/// <summary>
/// Предназначено для размещения утилитных функционалов работы с логами.
/// </summary>
namespace LogsWrapper
{
    /// <summary>
    /// Обёртка для логгера
    /// </summary>
    public static class Logger
    {
        private static ILog log;

        /// <summary>
        /// Обращение к обёрнутому логгеру log4net
        /// </summary>
        public static ILog Log
        {
            get
            {
                if (log == null)
                {
                    //XmlConfigurator.Configure();
                    log = LogManager.GetLogger("LOGGER");
                }

                return log;
            }
        }

        /// <summary>
        /// Инициализация логгера.
        /// </summary>
        public static void InitLogger()
        {
            XmlConfigurator.Configure();
            //log = LogManager.GetLogger("LOGGER");
        }
        
        /// <summary>
        /// Создаёт предупреждающее пользователя окно, параллельно делает 
        /// запись в системе логгирования.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFile"></param>
        /// <param name="numLine"></param>
        /// <returns></returns>
        public static MessageBoxResult MsgBox(string message, 
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "", 
            [System.Runtime.CompilerServices.CallerLineNumber] int numLine = 0)
        {
            Log.Warn($"{message} {memberName} {sourceFile} строка:{numLine}");
            return MessageBox.Show(message);
        }

        /// <summary>
        /// Логгирует со статусом DEBUG, при этом записывает откуда была вызвана 
        /// процедура в которой идёт логгирование.
        /// </summary>
        /// <param name = "message"></param>
        /// <param name = "memberName"></param>
        public static void Debug(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        {
            Log.Debug($"{message} {memberName} {sourceFile}");
        }
    }
}
