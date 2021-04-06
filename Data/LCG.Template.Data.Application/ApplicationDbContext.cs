using LCG.Template.Common.Entities.Application;
using LCG.Template.Common.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace LCG.Template.Data.Application
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly int MAX_STRING_INFO_LENGTH = 256;
        private static readonly int MAX_STRING_CONFIG_LENGTH = 1024;

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }


        #region ctors 
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext(string connectionString) : base(GetOptions(connectionString))
        {
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(e =>
            {
                e.HasKey(pk => pk.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Name).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.Website).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.HasOne(t => t.Language)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(fk => fk.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                e.ToTable("Accounts");
            });

            modelBuilder.Entity<EmailTemplateName>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Description).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.HasIndex(p => p.Description).IsUnique();
                e.ToTable("EmailTemplateNames");
            });

            modelBuilder.Entity<EmailTemplate>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Name).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.HasOne(t => t.EmailName).WithMany(p => p.EmailTemplates).HasForeignKey(fk => fk.EmailNameId);
                e.HasOne(t => t.Language).WithMany(p => p.EmailTemplates).HasForeignKey(fk => fk.LanguageId);
                e.ToTable("EmailTemplates");
            });

            modelBuilder.Entity<NotificationLog>(e => {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Subject).HasMaxLength(MAX_STRING_INFO_LENGTH * 2);
                e.Property(p => p.To).HasMaxLength(MAX_STRING_INFO_LENGTH * 2);
                e.Property(p => p.CC).HasMaxLength(MAX_STRING_INFO_LENGTH * 2);
                e.ToTable("NotificationLogs", "notification");
            });

            modelBuilder.Entity<AccountUser>(e =>
            {
                e.HasKey(e => new { e.AccountId, e.UserId });
                e.HasIndex(e => e.Id).IsUnique();
                e.Property(e => e.Id).ValueGeneratedOnAdd();
                e.Property(e => e.AccountPreferences).HasMaxLength(MAX_STRING_CONFIG_LENGTH).IsRequired(false);
                e.HasOne(e => e.Account)
                    .WithMany(e => e.AccountUsers)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(e => e.AccountUserType)
                    .WithMany(e => e.AccountUsers)
                    .HasForeignKey(e => e.AccountUserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(e => e.User)
                    .WithMany(e => e.AccountUsers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.ToTable("AccountUsers");
            });

            modelBuilder.Entity<AccountUserType>(e =>
            {
                e.HasKey(pk => pk.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.ToTable("AccountUserTypes");
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).ValueGeneratedNever();
                e.Property(p => p.Email).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.FirstName).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.MiddleNames).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.LastName).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.CompanyName).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.TaxNumber).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.PhoneNumber).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.ImageUrl).HasMaxLength(MAX_STRING_INFO_LENGTH);
                e.Property(p => p.LayoutPreferences).HasMaxLength(MAX_STRING_CONFIG_LENGTH);
                e.Property(p => p.Active).HasDefaultValue(true);
                e.Property(p => p.ImageUrl).HasDefaultValue("/assets/images/users/0.jpg");
                e.HasIndex(p => p.Email).IsUnique();
                e.ToTable("Users");
            });


            modelBuilder.Entity<Language>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).UseIdentityColumn();
                e.Property(p => p.Code).HasMaxLength(5);
                e.HasIndex(p => p.Code).IsUnique();
                e.ToTable("Languages");
            });
        }

        #region DBSets

        public DbSet<Language> LanguageSet { get; set; }
        public virtual DbSet<AccountUserType> AccountUserTypeSet { get; set; }
        public virtual DbSet<AccountUser> AccountUsersSet { get; set; }
        public virtual DbSet<Account> AccountsSet { get; set; }
        public virtual DbSet<User> UserSet { get; set; }
        public virtual DbSet<EmailTemplateName> EmailTemplateNameSet { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplateSet { get; set; }
        public virtual DbSet<NotificationLog> NotificationLogSet { get; set; }

        #endregion
    }
}
