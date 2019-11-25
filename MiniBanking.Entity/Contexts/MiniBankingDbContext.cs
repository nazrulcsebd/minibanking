using Microsoft.EntityFrameworkCore;

namespace MiniBanking.Entity
{
    public partial class MiniBankingDbContext : DbContext
    {
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAccount> CustomerAccount { get; set; }
        public virtual DbSet<AccountDeposit> AccountDeposit { get; set; }
        public virtual DbSet<AccountTransfer> AccountTransfer { get; set; }
        public virtual DbSet<AccountTransaction> AccountTransaction { get; set; }
        public virtual DbSet<AccountWithdrawal> AccountWithdrawal { get; set; }
        
        public MiniBankingDbContext(DbContextOptions<MiniBankingDbContext> options)
           : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("CustomerId");
            });

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("AccountId");
            });

            modelBuilder.Entity<AccountTransfer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("TransferId");
                entity.Ignore(b => b.CreatedOn);
                entity.Ignore(b => b.ModifiedOn);
            });

            modelBuilder.Entity<AccountTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("TransactionId");
                entity.Ignore(b => b.CreatedOn);
                entity.Ignore(b => b.ModifiedOn);
            });

            modelBuilder.Entity<AccountDeposit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("DepositId");
                entity.Ignore(b => b.CreatedOn);
                entity.Ignore(b => b.ModifiedOn);
            });

            modelBuilder.Entity<AccountWithdrawal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(p => p.Id).HasColumnName("WithdrawalId");
                entity.Ignore(b => b.CreatedOn);
                entity.Ignore(b => b.ModifiedOn);
            });
        }
    }
}
