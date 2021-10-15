using Bogdanov.SqlAnalyzerLib.Model;
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
        /// Поиск колонок в базе данных
        /// </summary>
        /// <returns></returns>
        Task<List<Column>> SearchColumnsInDbAsync(string db);
        /// <summary>
        /// Поиск колонок по всему SQL серверу
        /// </summary>
        /// <returns></returns>
        Task<List<Column>> SearchColumnsInSqlServerAsync();
        /// <summary>
        /// Поиск переменной в колонках таблиц базы данных.
        /// </summary>
        /// <param name="value">Значение, которое надо искать в таблицах</param>
        /// <param name="db">Наименование базы данных, в которой надо искать значение</param>
        /// <param name="isPartStr">Значение может являться частью строки, на int, 
        /// datetime, double данный функционал не должен распространяться. По-умолчанию,
        /// false.</param>
        /// <returns></returns>
        Task<SearchValueResult> SearchValueInColumnsDBAsync(
            string value, string db, bool isPartStr = false);
    }

    public class SearchValueResult
    {
        /// <summary>
        /// Список колонок, в которых хранится значение.
        /// </summary>
        public List<Column> ColumnsWithValue { get; set; }
    }
}
