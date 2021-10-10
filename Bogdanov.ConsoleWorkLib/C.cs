/// <summary>
/// Библиотека и пространство имён является утилитным и предназначено для унификации работы
/// с библиотекой консольных приложений.
/// </summary>
/// <remarks>
/// Предполагается, что консольное приложение будет придерживаться определённых
/// стандартов оформления при работе с библиотекой, например, так:
/// <code>
/// using VashBroker.CommonLibs.ConsoleWork;
///Func<bool> ExitConfirm = () =>
///{
///    string result = C.InterractH("Вы действительно хотите закрыть программу? Если да, введите y(yes) или Y(ES): ");
///    if (result.ToLower() == "yes" || result.ToLower() == "y")
///    {
///        return true;
///    }
///    return false;
///};
///C.InfoH("Добро пожаловать в тестовую консоль для библиотеки VashBroker.CommonLibs");
///Console.WriteLine();
///bool isExit = false;
///while (!isExit)
///{
///    C.InfoH("Номера разделов тестирования:");
///    C.InfoH("0 или <Enter> - выйти из тестирования");
///    C.InfoH("1 - тестирование раздела AltaListener, посвященного взаимодействию с базами данных Альта");
///    string testChapter = C.Interract("Введите номер раздела тестирования: ");
///    switch (testChapter)
///    {
///        case "0":
///            isExit = ExitConfirm();
///            break;
///        case "":
///            isExit = ExitConfirm();
///            break;
///        case "1":
///            AltaDBTests();
///            break;
///        default:
///            C.Warn("Номер не распознан. Введите корректный номер.");
///            break;
///    }
///}
///void AltaDBTests()
///{
///    //TODO: ...
///}
/// </code>
/// </remarks>
namespace Bogdanov.ConsoleWorkLib
{
    /// <summary>
    /// Класс для упрощённой ввода/вывода информации на консоль
    /// </summary>
    public static class C
    {
        #region public

        /// <summary>
        /// Выводит на экран информационные сообщения. Отличается от смысловой 
        /// нагрузки статуса логов, где наверно ближе к Warn.
        /// InfoH - info hight.
        /// </summary>
        /// <param name="msg"></param>
        public static void InfoH(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Выводит на экран информационные сообщения. Отличается от смысловой 
        /// нагрузки статуса логов, где наверно ближе к Debug.
        /// InfoL - info low.
        /// </summary>
        /// <param name="msg"></param>
        public static void InfoL(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Выводит на экран предупреждающие сообщения, которые не являются ошибкой.
        /// Например: некорректные действия пользователя, которые не приводят к падению
        /// программы.
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Показывает информацию об успешности действий или добавлении чего-то.
        /// </summary>
        /// <param name="msg"></param>
        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Показывает информацию об неуспешности действий или удалении чего-то.
        /// </summary>
        /// <param name="msg"></param>
        public static void UnSuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        /// <summary>
        /// Интерактивное взаимодействие. Предлагает ввести какие-либо данные.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Interract(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Интерактивное взаимодействие повышенной важности. Предлагает ввести какие-либо данные.
        /// InterractH - Hight Interraction.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string InterractH(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(msg);
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        #endregion
    }
}