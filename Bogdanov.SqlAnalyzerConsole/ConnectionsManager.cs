using System.Xml.Serialization;

namespace Bogdanov.SqlAnalyzerConsole
{
    /// <summary>
    /// Класс управления строками подключения и текущим подключением
    /// </summary>
    class ConnectionsManager : IDisposable
    {
        #region public

        public Connections? Connections { get; set; }

        IAnalyzeDbOrServer SearcherValueInDb { get; set; }

        public ConnectionsManager(IAnalyzeDbOrServer searcherValueInDb)
        {
            SearcherValueInDb = searcherValueInDb;
            LoadSavedConnections();
            SearcherValueInDb.ConnectionString = Connections?.CurrentConnection.Value.conStr ?? "";

        }

        /// <summary>
        /// Добавление строки подключения в список подключений.
        /// </summary>
        public void AddConnection()
        {
            string name = C.Interract($"Введите наименование подключения к " +
                $"{SearcherValueInDb.Description}: ");
            string con = C.Interract($"Введите строку подключения к " +
                $"{SearcherValueInDb.Description}: ");
            Connections.CurrentConnection = (name, con);
            SearcherValueInDb.ConnectionString = con;
            switch (SearcherValueInDb)
            {
                case MsSqlAnalyzeDbOrServer ms:
                    Connections.MsSqlConnections.Add((name, con));
                    break;
                case OracleAnalyzeDbOrServer or:
                    Connections.OracleConnections.Add((name, con));
                    break;
                case PostgreSqlAnalyzeDbOrServer pos:
                    Connections.PostgreSqlConnections.Add((name, con));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Выбор из списка подключений необходимой строки
        /// </summary>
        public void CheckConnection()
        {
            Action<List<(string name, string conStr)>> Connects = x =>
            {
                int i = 1;
                foreach (var item in x)
                {
                    C.InfoL($"{i} - {item.name}");
                    i++;
                }
                int numConnect = int.Parse(C.Interract("Выберите номер подключения: "));
                Connections.CurrentConnection = (x[numConnect - 1].name, 
                    x[numConnect - 1].conStr);
                SearcherValueInDb.ConnectionString = x[numConnect - 1].conStr;
                C.InfoL($"Выбрано подключение {Connections.CurrentConnection.Value.name}, " +
                    $"строка подключения {Connections.CurrentConnection.Value.conStr}");
            };

            C.InfoL("Доступны следующие подключения:");
            
            switch (SearcherValueInDb)
            {
                case MsSqlAnalyzeDbOrServer ms:
                    Connects(Connections.MsSqlConnections);
                    break;
                case OracleAnalyzeDbOrServer or:
                    Connects(Connections.OracleConnections);
                    break;
                case PostgreSqlAnalyzeDbOrServer pos:
                    Connects(Connections.PostgreSqlConnections);
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// Выводит список существующих подключений.
        /// </summary>
        public void ConnectionsList()
        {
            Action<List<(string name, string conStr)>> Connects = x =>
            {
                C.InfoL("На данный момент существуют следующие подключения:");
                if (x.Count == 0)
                {
                    C.InfoL("Созданных подключений нет!");
                    return;
                }
                foreach (var item in x)
                {
                    C.InfoL($"  {item.name}\t: {item.conStr}");
                }
            };

            switch (SearcherValueInDb)
            {
                case MsSqlAnalyzeDbOrServer ms:
                    Connects(Connections.MsSqlConnections);
                    break;
                case OracleAnalyzeDbOrServer or:
                    Connects(Connections.OracleConnections);
                    break;
                case PostgreSqlAnalyzeDbOrServer pos:
                    Connects(Connections.PostgreSqlConnections);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            SaveConnections();
        }

        #endregion

        #region private

        void LoadSavedConnections()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Connections));
            if (File.Exists("Connections.dat"))
            {
                using (FileStream fs = new FileStream("Connections.dat", FileMode.OpenOrCreate))
                {
                    Connections = serializer.Deserialize(fs) as Connections;
                }
            }
            else
            {
                Connections = new Connections();
            }
        }

        void SaveConnections()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Connections));
            using (FileStream fs = new FileStream("Connections.dat", FileMode.OpenOrCreate))
            {
                serializer.Serialize(fs, Connections);
            }
        }

        #endregion
    }
}
