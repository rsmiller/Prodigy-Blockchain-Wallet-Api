using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Database;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer
{
    public interface IWalletContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UserSession> UserSessions { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    }

    public class WalletContext : DbContext, IWalletContext
    {
        public IDatabaseConnectionSettings ConnectionSettings { get; set; }

        public WalletContext(IDatabaseConnectionSettings configuration)
        {
            this.ConnectionSettings = configuration;
        }

        public WalletContext(DbContextOptions<WalletContext> options, IDatabaseConnectionSettings configuration)
            : base(options)
        {
            this.ConnectionSettings = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseMySql(this.ConnectionSettings.ConnectionString, ServerVersion.AutoDetect(this.ConnectionSettings.ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionsConfiguration());

            base.OnModelCreating(modelBuilder);
        }


        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
    }
}
