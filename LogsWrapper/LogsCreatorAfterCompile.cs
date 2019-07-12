using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsWrapper
{
    /// <summary>
    /// Класс добавления в код логов после компиляции.
    /// </summary>
    /// <remarks>
    /// Класс должен выполнять следующие задачи:
    /// 1. Находить все методы в сборке помеченные атрибутом LoggedAttribute;
    /// 2. Вставлять в начало метода помеченного атрибутом LoggedAttribute строку:
    /// <code>
    /// Logger.Debug("Вошёл в метод");
    /// </code>
    /// 3. Вставлять в конец метода помеченного атрибутом LoggedAttribute строку:
    /// <code>
    /// Logger.Debug("Вышел из метода");
    /// </code>
    /// ??? 4. Вставлять в конструкции try..catch.. метода помеченного атрибутом 
    /// LoggerAttribute в блок catch (Exceptrin err) конструкцию:
    /// <code>
    /// Logger.Error(err);
    /// </code>
    /// (Непонятна необходимость в функциональности данной фичи!)
    /// </remarks>
    class LogsCreatorAfterCompile
    {
    }

    /// <summary>
    /// Атрибут, помечающий методы и свойства, которые необходимо обложить логами.
    /// </summary>
    public class LoggedAttribute : Attribute
    {

    }
}
