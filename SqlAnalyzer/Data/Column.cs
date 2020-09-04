using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAnalyzer.Data
{
    public class SearchServerContext : DbContext
    {
        public DbSet<DataBaseInServer> DataBasesInServer { get; set; }
        public DbSet<Column> Columns { get; set; }

        public SearchServerContext(string connectionString) : base(connectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Column>().HasKey(p =>
                new { p.TABLE_NAME, p.COLUMN_NAME });
            modelBuilder.Entity<DataBaseInServer>().HasKey(p =>
                new { p.Name });
        }
    }

    public class Column
    {
        //[Key]
        public string TABLE_NAME { get; set; }

        //[Key]
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

    public class DataBaseInServer
    {
        //[Key]
        public string Name { get; set; }
    }
}
