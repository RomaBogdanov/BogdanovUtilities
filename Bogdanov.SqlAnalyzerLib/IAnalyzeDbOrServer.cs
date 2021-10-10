using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.SqlAnalyzerLib
{
    /// <summary>
    /// Интерфейс базового поисковика по конкретному серверу конкретных значений 
    /// или их частей.
    /// На его базе должны появиться поисковики для MS SQL, Oracle, Postgres.
    /// </summary>
    public interface IAnalyzeDbOrServer
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// Описание поисковика данных в БД.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Поиск переменной в колонках таблиц базы данных.
        /// </summary>
        /// <param name="value">Значение, которое надо искать в таблицах</param>
        /// <param name="db">Наименование базы данных, в которой надо искать значение</param>
        /// <returns></returns>
        SearchValueResult SearchValueInColumnsDB(string value, string db);
    }

    public class SearchValueResult
    {
    }
}
