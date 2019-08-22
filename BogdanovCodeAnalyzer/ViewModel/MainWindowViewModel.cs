using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using BogdanovUtilitisLib.MVVMUtilsWrapper;
using BogdanovUtilitisLib.LogsWrapper;
using r = BogdanovUtilitisLib.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BogdanovCodeAnalyzer.ViewModel
{
    /// <summary>
    /// Обработчик формы анализатора
    /// </summary>
    /// <remarks>
    /// 8. Сравнивать результаты разных логов:
    /// 8.1. Смотреть сходства в логах;
    /// 8.2. Смотреть различия в логах;
    /// 9. Сохранять состояние по объектам:
    /// 9.1. Научиться находить неизменяемую часть состояний объектов;
    /// 9.2. Научиться находить изменяемую часть состояний объектов;
    /// 11. Научиться анализировать библиотеку:
    /// 11.1. Научиться строить объектную модель библиотеки (иерархическое дерево);
    /// 11.2. Научиться находить на иерархическом дереве требуемые типы;
    /// 11.3. Научиться находить минимальную иерархическую связь между типами;
    /// 11.4. Научиться находить связь между типами через публичные методы и свойства;
    /// 11.4.1. Тоже самое, с добавлением других методов и свойств - прайвт, протектод и т.д.;
    /// 11.4.2. Тоже самое, с учётом того, что эти типы являются наследниками типов более высокого уровня;
    /// 11.5. Научиться находить вызов типов изнутри методов.
    /// 12. Перенести данные по работе с IMAP сервером.
    /// </remarks>
    class MainWindowViewModel : NotifyPropertyChanged
    {


        public MainWindowViewModel()
        {
            
        }


    }

    /// <summary>
    /// Информация по логу.
    /// </summary>
    class Log
    {
        public string MsgLog { get; set; }
        public long Count { get; set; }
    }

    /// <summary>
    /// Обёртка для подключения к Базе данных
    /// </summary>
    class ConnectionDB
    {
        /// <summary>
        /// Выбрано ли подключение для исследования?
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// Статический класс для приёма сообщений от клиентов.
    /// </summary>
    static class AggregatorMessages
    {
        public static event Action<Message> OnMessageFromClient;

        public static void ReceiveMessage(Message message)
        {
            OnMessageFromClient?.Invoke(message);
        }
    }
}
