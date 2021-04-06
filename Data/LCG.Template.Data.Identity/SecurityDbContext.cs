using LCG.Template.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LCG.Template.Data.Identity
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        #region ctors
        public SecurityDbContext() : base()
        {
        }

        public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
            : base(options)
        {
        }

        public SecurityDbContext(string connectionString)
        : base(GetOptions(connectionString))
        {
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(e =>
            {
                e.Property(p => p.ApplicationUserId).UseIdentityColumn();
                e.Property(p => p.ApplicationUserId).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
        }
    }
}
