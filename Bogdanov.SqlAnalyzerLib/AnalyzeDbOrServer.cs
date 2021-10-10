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

        public abstract SearchValueResult SearchValueInColumnsDB(string value, string db);
        
    }

    /// <summary>
    /// Поисковик конкретных значений или их частей для MS SQL.
    /// </summary>
    public class MsSqlAnalyzeDbOrServer : AnalyzeDbOrServer
    {
        public override string Description => "поисковик(y) данных для MS SQL";

        public override SearchValueResult SearchValueInColumnsDB(string value, string db)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Поисковик конкретных значений или их частей для Oracle.
    /// </summary>
    public class OracleAnalyzeDbOrServer : AnalyzeDbOrServer
    {
        public override string Description => "поисковик(у) данных для Oracle";

        public override SearchValueResult SearchValueInColumnsDB(string value, string db)
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

        public override SearchValueResult SearchValueInColumnsDB(string value, string db)
        {
            throw new NotImplementedException();
        }
    }
}