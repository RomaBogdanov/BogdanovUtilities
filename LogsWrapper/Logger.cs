using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

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
        /// Создаёт предупреждающее пользователя окно, параллельно делает запись в системе логгирования
        /// </summary>
        /// <param name = "message"></param>
        /// <param name = "memberName"></param>
        /*public static MsgBoxResult MsgBox(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "", [System.Runtime.CompilerServices.CallerLineNumber] int numLine = 0)
        {
            return MsgBox(message, MsgBoxStyle.ApplicationModal, memberName, sourceFile, numLine);
            Log.Warn($"{message} {memberName} {sourceFile} строка:{numLine}");
                Interaction.MsgBox(message);
        }

        public static MsgBoxResult MsgBox(string message, MsgBoxStyle style, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "", [System.Runtime.CompilerServices.CallerLineNumber] int numLine = 0)
        {
            Log.Warn($"{message} {memberName} {sourceFile} строка:{numLine}");
            return Interaction.MsgBox(message, style);
        }*/

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
