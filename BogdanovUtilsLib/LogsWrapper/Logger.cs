using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using log4net;
using log4net.Config;

/// <summary>
/// Предназначено для размещения утилитных функционалов работы с логами.
/// </summary>
namespace BogdanovUtilsLib.LogsWrapper
{
    /// <summary>
    /// Обёртка для логгера
    /// </summary>
    /// <remarks>
    /// Реализованный функционал:
    /// 1. Возможность запустить логгер
    /// Для запуска логгера необходимо:
    /// Настроить конфигурационный файл. Например так:
    /// <code>
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration>
    ///  <configSections>
    ///    <section name = "log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    ///  </configSections>
    ///    <startup> 
    ///        <supportedRuntime version = "v4.0" sku=".NETFramework,Version=v4.6.1" />
    ///    </startup>
    ///  <log4net>
    ///    <appender name = "OutputAppender" type="log4net.Appender.ConsoleAppender">
    ///      <layout type = "log4net.Layout.PatternLayout" >
    ///        < conversionPattern value="%message%newline" />
    ///      </layout>
    ///          <appender name="LogFileAppender" type="log4net.Appender.RollingFileAppender">
    ///  <param name = "File" value="Logs\Log.log" />
    ///  <param name = "AppendToFile" value="true" />
    ///  <maxSizeRollBackups value = "10" />
    ///  < maximumFileSize value="5MB" />
    ///  <lockingModel type = "log4net.Appender.FileAppender+MinimalLock" />
    ///  < layout type="log4net.Layout.PatternLayout">
    ///    <param name = "ConversionPattern" value="%d  %-5p %m %n" />
    ///  </layout>
    ///</appender>
    ///<appender name = "LogFileAppenderExtention" type="log4net.Appender.RollingFileAppender">
    ///  <param name = "File" value="Logs\LogExtention.log" />
    ///  <param name = "AppendToFile" value="true" />
    ///  <maxSizeRollBackups value = "10" />
    ///  < maximumFileSize value="5MB" />
    ///  <lockingModel type = "log4net.Appender.FileAppender+MinimalLock" />
    ///  < layout type="log4net.Layout.PatternLayout">
    ///    <param name = "ConversionPattern" value="%d  %-5p %m %location%n" />
    ///  </layout>
    /// 
    ///    </appender>
    ///
    ///    <logger name = "LOGGER" >
    ///      <appender-ref ref="OutputAppender" />
    ///      <appender-ref ref="LogFileAppender" />
    ///      <appender-ref ref="LogFileAppenderExtention" />
    ///    </logger>
    ///  </log4net>
    /// </configuration>
    /// </code>
    /// Перед началом работы логгера сделать его инициализацию:
    /// <code>
    /// Logger.InitLogger();
    /// </code>
    /// 
    /// 2. Возможность логгирования со статусами:
    /// Debug
    /// Info
    /// Warn
    /// Error
    /// Fatal
    /// Примеры:
    /// <code>
    /// ...
    /// Logger.Log.Debug("message");
    /// Logger.Log.Info("message");
    /// Logger.Log.Warn("message");
    /// Logger.Log.Error("message");
    /// Logger.Log.Fatal("message");
    /// ...
    /// </code>
    /// 3. Возможность быстрой записи логга в статусах:
    /// Debug
    /// Error
    /// Примеры:
    /// <code>
    /// ...
    /// Logger.Error("message");
    /// Logger.Debug("message");
    /// ...
    /// </code>
    /// 4. Возможность записи лога в статусе Warn плюс выведения формы 
    /// предупреждения пользователю на экран.
    /// Примеры:
    /// <code>
    /// ...
    /// Logger.MsgBox("")
    /// ...
    /// </code>
    /// Требуемый к реализации функционал:
    /// 1. Логгировать полное состояние объекта в конкретный момент времени.
    /// 2. Выводить разность в состояниях объектов одного типа.
    /// </remarks>
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
        public static void Debug(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        {
            Log.Debug($"{message} {memberName} {sourceFile}");
        }

        /// <summary>
        /// Логгирует со статусом Error, при этом записывает откуда была вызвана 
        /// процедура в которой идёт логгирование.
        /// </summary>
        /// <param name = "message"></param>
        /// <param name = "memberName"></param>
        public static void Error(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        {
            Log.Error($"{message} {memberName} {sourceFile}");
        }

        /// <summary>
        /// Логгирует со статусом Error, при этом записывает откуда была вызвана 
        /// процедура в которой идёт логгирование.
        /// </summary>
        /// <param name = "message"></param>
        /// <param name = "memberName"></param>
        public static void Error(Exception exception,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        {
            Log.Error($"{exception.Message} {memberName} {sourceFile}", exception);
        }
    }

}
