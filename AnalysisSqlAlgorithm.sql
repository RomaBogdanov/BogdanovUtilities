-- Получаем список баз данных с сортировкой по размерам БД
select d.name, sum(m.size) full_size from sys.databases d
join sys.master_files m on d.database_id = m.database_id
where type_desc='rows'
group by d.name
order by full_size desc

-- Получаем количество строк во всех таблицах всех БД сервера отсортированное по количеству строк
exec sp_MSforeachdb 'use ?
select OBJECT_NAME(id) ?, rows from sysindexes order by rows desc'

/* Если в этом нет необходимости, то можем сделать получение количетсва строк в таблицах по конкретной БД.
Например:
use gtd2011
select distinct OBJECT_NAME(id) tab, rows from sysindexes
order by rows desc
*/

-- По основным таблицам получаем список колонок с основной информацией
select COLUMN_NAME, DATA_TYPE, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='EDMsgs'