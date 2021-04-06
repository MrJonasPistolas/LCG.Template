using LCG.Template.Common.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LCG.Template.Data.Logging
{
    public class LoggerDbContext : DbContext
    {
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        #region ctors 
        public LoggerDbContext() : base()
        {
        }

        public LoggerDbContext(DbContextOptions<LoggerDbContext> options)
        : base(options)
        {
        }

        public LoggerDbContext(string connectionString)
        : base(GetOptions(connectionString))
        {
        }
        #endregion

        #region DbSets
        public DbSet<EventLog> EventLogSet { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<EventLog>(e =>
            {
                e.HasKey(pk => new { pk.Id, pk.SystemLog });
                e.Property(p => p.Id).UseIdentityColumn();
                e.ToTable("AspNetEventLog");
            });

        }
    }
}
