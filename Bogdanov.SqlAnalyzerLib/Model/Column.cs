using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.SqlAnalyzerLib.Model
{
    /// <summary>
    /// Содержит описание колонки из таблицы базы данных.
    /// </summary>
    public class Column
    {
        public string TABLE_NAME { get; set; }

        public string COLUMN_NAME { get; set; }

        public string TABLE_CATALOG { get; set; }

        public string DATA_TYPE { get; set; }

        /// <summary>
        /// Пример записи в колонке.
        /// </summary>
        [NotMapped]
        public string SampleValue { get; set; }

        /// <summary>
        /// Количество записей в колонке.
        /// </summary>
        [NotMapped]
        public long CountRecsInColumn { get; set; }

        /// <summary>
        /// Количество уникальных записей в колонке.
        /// </summary>
        [NotMapped]
        public long CountUniqueRecsInColumn { get; set; }

        /// <summary>
        /// Указывает, что скачивание детализации по столбцу было сделано с ошибкой.
        /// </summary>
        [NotMapped]
        public bool IsError { get; set; }

        /// <summary>
        /// Указывает ошибку при скачивании детализации по столбцу.
        /// </summary>
        [NotMapped]
        public string ErrorMsg { get; set; }
    }
}
