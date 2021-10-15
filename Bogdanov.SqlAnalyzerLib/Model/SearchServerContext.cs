using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.SqlAnalyzerLib.Model
{
    public class SearchServerContext : DbContext
    {
        string _connectionString;

        public DbSet<DataBase> DataBasesInServer { get; set; }
        public DbSet<Column> Columns { get; set; }

        public SearchServerContext(string connectionString) : base()
        {
            _connectionString  = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Column>().HasKey(p =>
            //    new { p.TABLE_NAME, p.COLUMN_NAME });
            modelBuilder.Entity<Column>().HasNoKey();
        }
    }
}
