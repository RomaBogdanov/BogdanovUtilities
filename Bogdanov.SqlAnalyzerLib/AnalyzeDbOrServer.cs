using Bogdanov.SqlAnalyzerLib.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bogdanov.SqlAnalyzerLib
{
    /// <summary>
    /// Базовый поисковик по конкретному серверу конкретных значений или их частей.
    /// На его базе должны появиться поисковики для MS SQL, Oracle, Postgres.
    /// </summary>
    public abstract class AnalyzeDbOrServer : IAnalyzeDbOrServer
    {

        private string connectionString = "";

        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public abstract string Description { get; }

        public abstract Task<List<Column>> SearchColumnsInDbAsync(string db);

        public abstract Task<List<Column>> SearchColumnsInSqlServerAsync();

        public abstract Task<SearchValueResult> SearchValueInColumnsDBAsync(
            string value, string db, bool isPartStr = false);
    }

    /// <summary>
    /// Поисковик конкретных значений или их частей для MS SQL.
    /// </summary>
    public class MsSqlAnalyzeDbOrServer : AnalyzeDbOrServer
    {
        public override string Description => "поисковик(y) данных для MS SQL";

        public override async Task<List<Column>> SearchColumnsInDbAsync(string db)
        {
            return await Task.Run(async () =>
            {
                string query = "select TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, " +
                    $"DATA_TYPE from {db}.INFORMATION_SCHEMA.COLUMNS";
                using (SearchServerContext dbs = new SearchServerContext(
                    ConnectionString))
                {
                    return dbs.Columns.FromSqlRaw<Column>(query).ToList();
                }
            });
        }

        public override async Task<List<Column>> SearchColumnsInSqlServerAsync()
        {
            string query = "select name from sys.databases " +
                "where name not in ('master', 'tempdb', 'model', 'msdb')";
            List<Column> columns = new List<Column>();
            await Task.Run(async () =>
            {
                using (SearchServerContext dbs = new SearchServerContext(ConnectionString))
                {
                    var dataBases = dbs.DataBasesInServer.FromSqlRaw<DataBase>
                        (query).ToList();
                    await foreach (var item in GetAllColumnsInServer(dataBases))
                    {
                        columns.AddRange(item);
                    }
                }
            });
            return columns;
        }

        /// <summary>
        /// Поиск переменной в колонках базы данных
        /// </summary>
        /// <param name="value"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        /// <remarks>
        /// Для изучения вопросов типов данных в SQL и C# и их соотношений 
        /// рекомендуются следующие статьи:
        /// <see cref="https://docs.microsoft.com/ru-ru/sql/t-sql/data-types/data-types-transact-sql?view=sql-server-ver15"/>
        /// <see cref="https://metanit.com/sharp/adonetcore/2.8.php"/>
        /// В данной процедуре происходит поиск не по всем типам данных
        /// (заполнять по мере необходимости).
        /// </remarks>
        public override async Task<SearchValueResult> SearchValueInColumnsDBAsync(
            string value, string db, bool isPartStr = false)
        {
            Func<Column, string, bool, string> getQuery = (Column colData,
                string value, bool isPartStr) =>
           {
               if (isPartStr)
               {
                   return $"select count([{colData.COLUMN_NAME}]) from " +
                    $"{colData.TABLE_CATALOG}.dbo.[{colData.TABLE_NAME}] " +
                    $"where [{colData.COLUMN_NAME}] like '%{value}%'";
               }
               return $"select count([{colData.COLUMN_NAME}]) from " +
                $"{colData.TABLE_CATALOG}.dbo.[{colData.TABLE_NAME}] " +
                $"where [{colData.COLUMN_NAME}] = '{value}'";
           };
            List<Column> resultColumns = new List<Column>();
            List<Column>? cols = await SearchColumnsInDbAsync(db);
            bool isParsedAsDateTime = DateTime.TryParse(value, out DateTime parsedDT);
            bool isParsedAsDouble = Double.TryParse(value, out double parsedDouble);
            bool isParsedAsInt = int.TryParse(value, out int parsedInt);
            //TODO: здесь надо экспериментировать
            //timestamp не обрабатывается
            if (isParsedAsDateTime)
            {
                //TODO: реализовать
                //date, datetime, datetime2, smalldatetime

            }
            else if (isParsedAsDouble)
            {
                //TODO: реализовать

            }
            else if (isParsedAsInt)
            {
                //TODO: реализовать

            }
            else
            {
                // text, ntext, image будут исключены из версий SQL после 2021 года,
                // поэтому их не рекомендуется использовать и я их исключаю,
                // хотя с точки зрения взаимодействия со старыми базами данных это
                // может быть неверно.
                List<string> sqlStringTypes = new List<string>() // Типы в MS SQL, которые преобразуются в строку.
                {
                    "char", "varchar", "nchar", "nvarchar"
                };
                // Используем ADO.NET, потому что EF не предоставляет
                // нормального функционала для выполнения задачи.
                using (var sc = new SqlConnection(ConnectionString))
                {
                    foreach (var col in cols.Where(x => sqlStringTypes.Contains(
                        x.DATA_TYPE)))
                    {
                        sc.Open();
                        string query = getQuery(col, value, isPartStr);
                        using (var command = new SqlCommand(query, sc))
                        {
                            try
                            {
                                using (var res = command.ExecuteReader())
                                {
                                    while (res.Read())
                                    {
                                        int count = Convert.ToInt32(res[0]);
                                        if (count > 0)
                                        {
                                            col.CountRecsInColumn = count;
                                            resultColumns.Add(col);
                                        }

                                    }
                                }
                            }
                            catch (Exception err)
                            {
                                // TODO: добавить обработчик ошибок, в частности, логгирование или событие, на которое можно подписаться.
                            }
                        }

                        sc.Close();

                    }
                }
            }
            return new SearchValueResult { ColumnsWithValue = resultColumns };
        }

        #region private

        private async IAsyncEnumerable<List<Column>> GetAllColumnsInServer(
            List<DataBase> dataBases)
        {
            foreach (var db in dataBases)
            {
                yield return await SearchColumnsInDbAsync(db.Name);
            }
        }

        #endregion
    }

    /// <summary>
    /// Поисковик конкретных значений или их частей для Oracle.
    /// </summary>
    public class OracleAnalyzeDbOrServer : AnalyzeDbOrServer
    {
        public override string Description => "поисковик(у) данных для Oracle";

        public override Task<List<Column>> SearchColumnsInDbAsync(string db)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Column>> SearchColumnsInSqlServerAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<SearchValueResult> SearchValueInColumnsDBAsync(
            string value, string db, bool isPartStr = false)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Поисковик конкретных значений или их частей для PostgreSQL.
    /// </summary>
    public class PostgreSqlAnalyzeDbOrServer : AnalyzeDbOrServer
    {
        public override string Description => "поисковик(у) данных для PostgreSQL";

        public override Task<List<Column>> SearchColumnsInDbAsync(string db)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Column>> SearchColumnsInSqlServerAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<SearchValueResult> SearchValueInColumnsDBAsync(
            string value, string db, bool isPartStr = false)
        {
            throw new NotImplementedException();
        }
    }

    public class TestString
    {

    }
}