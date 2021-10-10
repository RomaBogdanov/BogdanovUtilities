using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.SqlAnalyzerConsole
{
    /// <summary>
    /// Список подключений.
    /// </summary>
    [Serializable]
    public class Connections
    {
        private (string, string)? currentConnection = ("", "");

        public (string name, string conStr)? CurrentConnection
        {
            get => currentConnection;
            set => currentConnection = value;
        }
        public List<(string, string)> MsSqlConnections { get; set; } = 
            new List<(string, string)>();
        public List<(string, string)> OracleConnections { get; set; } = 
            new List<(string, string)>();
        public List<(string, string)> PostgreSqlConnections { get; set; } = 
            new List<(string, string)>();
    }
}
