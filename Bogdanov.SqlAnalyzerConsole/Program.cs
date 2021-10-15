// See https://aka.ms/new-console-template for more information
global using Bogdanov.ConsoleWorkLib;
global using Bogdanov.SqlAnalyzerLib;
using Bogdanov.SqlAnalyzerConsole;

IAnalyzeDbOrServer? searcherValue = null;

C.InfoH("Добро пожаловать в проект по анализу БД и Серверов SQL");

var descript = C.Interract("Вывести описание программы? (введите y - если, да):");
if (descript.ToLower() == "y")
{
    C.InfoL(File.ReadAllText("Description.txt"));
    C.InterractH("Нажмите Enter, чтобы продолжить");
}

C.InfoH("Проект поддерживает работу со следующими СУБД:");
C.InfoH("1 - MS SQL");
C.InfoH("2 - Oracle");
C.InfoH("3 - PostgreSQL");
CheckDB:
string dbType = C.Interract("Выберите номер типа баз данных, " +
    "с которыми предстоит работать: ");
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
C.InfoH("1 - поиск всех таблиц и их колонок в конкретной базе данных");
C.InfoH("2 - поиск всех таблиц и их колонок на всём сервере");
C.InfoH("3 - поиск таблиц и их колонок, которые содержат значение");

switch (C.Interract("Ввести номер задачи: "))
{
    case "1":
        string db = C.Interract("Введите наименование базы данных: ");
        var columns = searcherValue?.SearchColumnsInDbAsync(db).Result;
        foreach (var col in columns)
        {
            C.InfoL($"{col.TABLE_NAME}\t {col.COLUMN_NAME}\t " +
                $"{col.DATA_TYPE}\t {col.TABLE_CATALOG}");
        }
        break;
    case "2":
        var columnsAllDbs = searcherValue?.SearchColumnsInSqlServerAsync().Result;
        foreach (var col in columnsAllDbs)
        {
            C.InfoL($"{col.TABLE_NAME}\t {col.COLUMN_NAME}\t " +
                $"{col.DATA_TYPE}\t {col.TABLE_CATALOG}");
        }
        string details = C.Interract("Вывести агрегированную информацию?(y - если да):");
        if (details == "y")
        {
            C.Success("Уникальные типы данных, которые встречаются в базах сервера:");
            foreach (var col in columnsAllDbs.Select(x => x.DATA_TYPE)
                .Distinct().OrderBy(i => i))
            {
                C.InfoL(col);
            }
        }
        break;
    case "3":
        string db1 = C.Interract("Введите наименование базы данных: ");
        string val = C.Interract("Введите значение для поиска: ");
        C.InfoH("!!! Важное разъяснение: значение может искаться в ячейках " +
            "целиком, а может, как часть значения в ячейке. Например: Рома - " +
            "в первом случае будет искать все ячейки со значением Рома, " +
            "во втором будет учитывать и ячейки, где данное значение является" +
            "частью, например, Роман Богданов");
        string isPart = C.Interract("Значение может быть частью значения " +
            "в ячейке (елси да, введите y) :");
        bool isPartBool = isPart.ToLower() == "y" ? true : false;
        SearchValueResult? searchValue = searcherValue?
            .SearchValueInColumnsDBAsync(val, db1, isPartBool).Result;
        foreach (var item in searchValue.ColumnsWithValue)
        {
            C.InfoL($"{item.TABLE_NAME}\t {item.COLUMN_NAME}\t " +
                $"{item.CountRecsInColumn}");
        }
        C.Success("Задача завершена");
        break;
    default:
        break;
}
connectionsManager.Dispose();
