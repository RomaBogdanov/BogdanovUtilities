// See https://aka.ms/new-console-template for more information
global using Bogdanov.ConsoleWorkLib;
global using Bogdanov.SqlAnalyzerLib;
using Bogdanov.SqlAnalyzerConsole;

IAnalyzeDbOrServer? searcherValue = null;

C.InfoH("Добро пожаловать в проект по анализу БД и Серверов SQL");
C.InfoH("Проект поддерживает работу со следующими СУБД:");
C.InfoH("1 - MS SQL");
C.InfoH("2 - Oracle");
C.InfoH("3 - PostgreSQL");
CheckDB:
string dbType = C.Interract("Выберите номер типа баз данных, с которыми предстоит работать: ");
switch (dbType)
{
    case "1":
        searcherValue = new MsSqlAnalyzeDbOrServer();
        break;
    case "2":
        searcherValue = new OracleAnalyzeDbOrServer();
        break;
    case "3":
        searcherValue = new PostgreSqlAnalyzeDbOrServer();
        break;
    default:
        C.InfoH("Выбранного вами типа баз данных не существует. Попробуйте ещё раз.");
        goto CheckDB;
}
var varOfDb = searcherValue?.Description ?? "";
C.InfoH($"Вы выбрали { varOfDb }");
C.InfoH("");

ConnectionsManager connectionsManager = new ConnectionsManager(searcherValue);
connectionsManager.ConnectionsList();
C.InfoH("Выберите подключение. Либо введите новое, либо выберите из существующих");
C.InfoH("1 - ввести новую строку подключения");
C.InfoH("2 - выбрать строку подключения из существующих");

switch (C.Interract("Введите тип подлкючения: "))
{
    case "1":
        connectionsManager.AddConnection();
        break;
    case "2":
        connectionsManager.CheckConnection();
        break;
    default:
        break;
}

C.InfoH("Задачи, которые можно выполнить:");
C.InfoH("1 - поиск таблиц и их колонок, которые содержат значение");

switch (C.Interract("Ввести номер задачи: "))
{
    case "1":
        string db = C.Interract("Введите наименование базы данных: ");
        
        break;
    default:
        break;
}
connectionsManager.Dispose();
