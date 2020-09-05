using System.Data.Entity;

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
}
