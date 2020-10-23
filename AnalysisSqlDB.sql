-- Код большей частью взят с хабра https://habr.com/ru/post/241079/
-- страница с недокументированными функциями https://www.sql.ru/articles/mssql/02030101undocumentedsql.shtml

-- Имена сервера и экземпляра
select @@SERVERNAME as [server\instance]

-- Версия SQL Server
select @@VERSION

-- Экземпляр SQL Server
select @@SERVICENAME

-- Текущая БД, в контексте которой выполняется запрос
select DB_NAME()

-- Время работы с последнего запуска
select @@Servername AS ServerName ,
        create_date AS  ServerStarted ,
        DATEDIFF(s, create_date, GETDATE()) / 86400.0 AS DaysRunning ,
        DATEDIFF(s, create_date, GETDATE()) AS SecondsRunnig from sys.databases where name='tempdb'

-- Анализ связанных серверов. Там несколько разные данные данные. Объединить их попозже.
-- =============================================================
exec sp_helpserver;

exec sp_linkedservers;

SELECT  @@SERVERNAME AS Server ,
        Server_Id AS  LinkedServerID ,
        name AS LinkedServer ,
        Product ,
        Provider ,
        Data_Source ,
        Modify_Date
FROM    sys.servers
ORDER BY name; 

select * from sys.servers
-- =============================================================

-- Список баз данных. Позже сделать объединение данных.
-- =============================================================
exec sp_helpdb;

exec sp_databases;

SELECT  @@SERVERNAME AS Server ,
        name AS DBName ,
        recovery_model_Desc AS RecoveryModel ,
        Compatibility_level AS CompatiblityLevel ,
        create_date ,
        state_desc
FROM    sys.databases
ORDER BY Name; 

SELECT  @@SERVERNAME AS Server ,
        d.name AS DBName ,
        create_date ,
        compatibility_level ,
        m.physical_name AS FileName
FROM    sys.databases d
        JOIN sys.master_files m ON d.database_id = m.database_id
WHERE   m.[type] = 0 -- data files only
ORDER BY d.name; 

-- Получение списка БД отсортированного по размерам БД
select d.name, sum(m.size) full_size from sys.databases d
join sys.master_files m on d.database_id=m.database_id
where type_desc='rows'
group by d.name
order by full_size desc

-- =============================================================

-- Изучаем бэкапы
-- =============================================================

SELECT  @@Servername AS ServerName ,
        d.Name AS DBName ,
        MAX(b.backup_finish_date) AS LastBackupCompleted
FROM    sys.databases d
        LEFT OUTER JOIN msdb..backupset b
                    ON b.database_name = d.name
                       AND b.[type] = 'D'
GROUP BY d.Name
ORDER BY d.Name;

SELECT  @@Servername AS ServerName ,
        d.Name AS DBName ,
        b.Backup_finish_date ,
        bmf.Physical_Device_name
FROM    sys.databases d
        INNER JOIN msdb..backupset b ON b.database_name = d.name
                                        AND b.[type] = 'D'
        INNER JOIN msdb.dbo.backupmediafamily bmf ON b.media_set_id = bmf.media_set_id
ORDER BY d.NAME ,
        b.Backup_finish_date DESC; 
-- =============================================================

-- Активные пользовательские соединения. Разобраться.
-- =============================================================
select * from sys.dm_exec_sessions;

exec sp_who;
exec sp_who2;
-- =============================================================

-- Анализ баз данных.
-- =============================================================

-- Обозначения типов
--AF = статистическая функция (среда CLR);
--C = ограничение CHECK;
--D = DEFAULT (ограничение или изолированный);
--F = ограничение FOREIGN KEY;
--PK = ограничение PRIMARY KEY;
--P = хранимая процедура SQL;
--PC = хранимая процедура сборки (среда CLR);
--FN = скалярная функция SQL;
--FS = скалярная функция сборки (среда CLR);
--FT = возвращающая табличное значение функция сборки (среда CLR);
--R = правило (старый стиль, изолированный);
--RF = процедура фильтра репликации;
--S = системная базовая таблица;
--SN = синоним;
--SQ = очередь обслуживания;
--TA = триггер DML сборки (среда CLR);
--TR = триггер DML SQL;
--IF = встроенная возвращающая табличное значение функция SQL;
--TF = возвращающая табличное значение функция SQL;
--U = таблица (пользовательская);
--UQ = ограничение UNIQUE;
--V = представление;
--X = расширенная хранимая процедура;
--IT = внутренняя таблица.

-- Не забываем проставить базу данных, по которой будет проходить исследование.
--USE MyDatabase;
--GO

select * from sys.objects where type='U';
-- =============================================================

-- Изучение расположения файлов базы данных. Не забывай выставлять необходимую БД
exec sp_helpfile

SELECT  @@Servername AS Server ,
        DB_NAME() AS DB_Name ,
        File_id ,
        Type_desc ,
        Name ,
        LEFT(Physical_Name, 1) AS Drive ,
        Physical_Name ,
        RIGHT(physical_name, 3) AS Ext ,
        Size ,
        Growth
FROM    sys.database_files
ORDER BY File_id; 

-- Полный списко таблиц в БД
-- =============================================================
exec sp_tables; -- возвращает и таблицы и представления

SELECT  @@Servername AS ServerName ,
        TABLE_CATALOG ,
        TABLE_SCHEMA ,
        TABLE_NAME
FROM     INFORMATION_SCHEMA.TABLES
WHERE   TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME ;

SELECT  @@Servername AS ServerName ,
        DB_NAME() AS DBName ,
        o.name AS 'TableName' ,
        o.[Type] ,
        o.create_date
FROM    sys.objects o
WHERE   o.Type = 'U' -- User table 
ORDER BY o.name;

SELECT  @@Servername AS ServerName ,
        DB_NAME() AS DBName ,
        t.Name AS TableName,
        t.[Type],
        t.create_date
FROM    sys.tables t
ORDER BY t.Name;

-- Получение отсортированного количества записей в каждой таблице БД
select OBJECT_NAME(id) tab, rows from sysindexes
order by rows desc

-- Как в случае выше, но для всех баз данных сервера
exec sp_MSforeachdb 'use ?
select OBJECT_NAME(id) ?, rows from sysindexes order by rows desc'

-- =============================================================

-- Работа с колонками таблицы
-- =============================================================
-- Получение списка колонок с основной информацией
select COLUMN_NAME, DATA_TYPE, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='EDMsgs'


-- =============================================================

-- Генерация скрипта для подсчёта количества записей в таблицах БД. Подумать, куда ещё можно приложить генерацию запросов.
SELECT  'Select ''' + DB_NAME() + '.' + SCHEMA_NAME(SCHEMA_ID) + '.'
        + LEFT(o.name, 128) + ''' as DBName, count(*) as Count From ' + SCHEMA_NAME(SCHEMA_ID) + '.' + o.name
        + ';' AS ' Script generator to get counts for all tables'
FROM    sys.objects o
WHERE   o.[type] = 'U'
ORDER BY o.name;

-- Очень две интересные функции, которые позволяют делать запрос по отошению к каждой таблице или к каждой базе данных
-- Разобраться с этими функциями.
-- =============================================================

--exec sp_MSforeachtable 'select count(*) from ?'
--exec sp_MSforeachdb 'SELECT  @@Servername AS Server ,
--        DB_NAME() AS DB_Name ,
--        File_id ,
--        Type_desc ,
--        Name ,
--        LEFT(Physical_Name, 1) AS Drive ,
--        Physical_Name ,
--        RIGHT(physical_name, 3) AS Ext ,
--        Size ,
--        Growth
--FROM    sys.database_files
--ORDER BY File_id;'

-- Пример с выводом файловой информации по каждой базе
exec sp_MSForeachdb 'use *; exec sp_helpfile', @replacechar = '*'

-- =============================================================

-- Разобрать
-- =============================================================

-- Кучи (метод 1)

--SELECT  @@Servername AS ServerName ,
--        DB_NAME() AS DBName ,
--        t.Name AS HeapTable ,
--        t.Create_Date
--FROM    sys.tables t
--        INNER JOIN sys.indexes i ON t.object_id = i.object_id
--                                    AND i.type_desc = 'HEAP'
--ORDER BY t.Name 

----OR 
---- Кучи (Метод 2) 

--SELECT  @@Servername AS ServerName ,
--        DB_NAME() AS DBName ,
--        t.Name AS HeapTable ,
--        t.Create_Date
--FROM    sys.tables t
--WHERE    OBJECTPROPERTY(OBJECT_ID, 'TableHasClustIndex') = 0
--ORDER BY t.Name; 

----OR 
---- Кучи (Метод 3) + количество записей

--SELECT  @@ServerName AS Server ,
--        DB_NAME() AS DBName ,
--        OBJECT_SCHEMA_NAME(ddps.object_id) AS SchemaName ,
--        OBJECT_NAME(ddps.object_id) AS TableName ,
--        i.Type_Desc ,
--        SUM(ddps.row_count) AS Rows
--FROM    sys.dm_db_partition_stats AS ddps
--        JOIN sys.indexes i ON i.object_id = ddps.object_id
--                              AND i.index_id = ddps.index_id
--WHERE   i.type_desc = 'HEAP'
--        AND OBJECT_SCHEMA_NAME(ddps.object_id) <> 'sys'
--GROUP BY ddps.object_id ,
--        i.type_desc
--ORDER BY TableName;
-- =============================================================

-- !!! ОЧень интересно. Помогает узнать частоту использования таблиц в БД.
-- Чтение/запись таблицы
-- Кучи не рассматриваются, у них нет индексов
-- Только те таблицы, к которым обращались после запуска SQL Server
SELECT  @@ServerName AS ServerName ,
        DB_NAME() AS DBName ,
        OBJECT_NAME(ddius.object_id) AS TableName ,
        SUM(ddius.user_seeks + ddius.user_scans + ddius.user_lookups)
                                                               AS  Reads ,
        SUM(ddius.user_updates) AS Writes ,
        SUM(ddius.user_seeks + ddius.user_scans + ddius.user_lookups
            + ddius.user_updates) AS [Reads&Writes] ,
        ( SELECT    DATEDIFF(s, create_date, GETDATE()) / 86400.0
          FROM      master.sys.databases
          WHERE     name = 'tempdb'
        ) AS SampleDays ,
        ( SELECT    DATEDIFF(s, create_date, GETDATE()) AS SecoundsRunnig
          FROM      master.sys.databases
          WHERE     name = 'tempdb'
        ) AS SampleSeconds
FROM    sys.dm_db_index_usage_stats ddius
        INNER JOIN sys.indexes i ON ddius.object_id = i.object_id
                                     AND i.index_id = ddius.index_id
WHERE    OBJECTPROPERTY(ddius.object_id, 'IsUserTable') = 1
        AND ddius.database_id = DB_ID()
GROUP BY OBJECT_NAME(ddius.object_id)
ORDER BY [Reads&Writes] DESC;

GO

-- Операции чтения и записи
-- Кучи пропущены, у них нет индексов
-- Только таблицы, использовавшиеся после перезапуска SQL Server
-- В запросе используется курсор для получения информации во всех БД
-- Единый отчёт, хранится в tempdb

DECLARE DBNameCursor CURSOR
FOR
    SELECT  Name
    FROM    sys.databases
    WHERE    Name NOT IN ( 'master', 'model', 'msdb', 'tempdb',
                            'distribution' )
    ORDER BY Name; 

DECLARE @DBName NVARCHAR(128) 

DECLARE @cmd VARCHAR(4000) 

IF OBJECT_ID(N'tempdb..TempResults') IS NOT NULL
    BEGIN 
        DROP TABLE tempdb..TempResults 
    END 

CREATE TABLE tempdb..TempResults
    (
      ServerName NVARCHAR(128) ,
      DBName NVARCHAR(128) ,
      TableName NVARCHAR(128) ,
      Reads INT ,
      Writes INT ,
      ReadsWrites INT ,
      SampleDays DECIMAL(18, 8) ,
      SampleSeconds INT
    ) 

OPEN DBNameCursor 

FETCH NEXT FROM DBNameCursor INTO @DBName 
WHILE @@fetch_status = 0
    BEGIN 

---------------------------------------------------- 
-- Print @DBName 

        SELECT   @cmd = 'Use ' + @DBName + '; ' 
        SELECT   @cmd = @cmd + ' Insert Into tempdb..TempResults 
SELECT @@ServerName AS ServerName, 
DB_NAME() AS DBName, 
object_name(ddius.object_id) AS TableName , 
SUM(ddius.user_seeks 
+ ddius.user_scans 
+ ddius.user_lookups) AS Reads, 
SUM(ddius.user_updates) as Writes, 
SUM(ddius.user_seeks 
+ ddius.user_scans 
+ ddius.user_lookups 
+ ddius.user_updates) as ReadsWrites, 
(SELECT datediff(s,create_date, GETDATE()) / 86400.0 
FROM sys.databases WHERE name = ''tempdb'') AS SampleDays, 
(SELECT datediff(s,create_date, GETDATE()) 
FROM sys.databases WHERE name = ''tempdb'') as SampleSeconds 
FROM sys.dm_db_index_usage_stats ddius 
INNER JOIN sys.indexes i
ON ddius.object_id = i.object_id 
AND i.index_id = ddius.index_id 
WHERE objectproperty(ddius.object_id,''IsUserTable'') = 1 --True 
AND ddius.database_id = db_id() 
GROUP BY object_name(ddius.object_id) 
ORDER BY ReadsWrites DESC;' 

--PRINT @cmd 
        EXECUTE (@cmd) 

----------------------------------------------------- 

        FETCH NEXT FROM DBNameCursor INTO @DBName 
    END 

CLOSE DBNameCursor 

DEALLOCATE DBNameCursor 

SELECT  *
FROM    tempdb..TempResults
ORDER BY DBName ,
        TableName; 
--DROP TABLE tempdb..TempResults;

-- Нахождение зависимостей в БД
exec sp_MSdependencies null

-- Этот запрос использовать для анализа чтения записей в разных БД
select * from tempdb.dbo.TempResults order by Reads desc
