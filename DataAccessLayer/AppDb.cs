using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options)
        {

        }
        public List<Transaction> GetAll()
        {
            return this.Transaction.ToList();
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }
        #region DbSet
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<AllTransactions> AllTransactions { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().HasNoKey();
            modelBuilder.Entity<Payment>().HasNoKey();
            modelBuilder.Entity<Category>().HasNoKey();
            base.OnModelCreating(modelBuilder);

        }
    }



}