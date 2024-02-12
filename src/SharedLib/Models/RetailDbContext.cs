using Microsoft.EntityFrameworkCore;

namespace SharedLib.Models
{
    public class RetailDbContext : DbContext
    {
        public RetailDbContext(DbContextOptions<RetailDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; init; }

        //public DbSet<Customer> Customer { get; init; }
        //public DbSet<SalesOrder> SalesOrders { get; init; }
        //public DbSet<Message> Messages { get; init; }
        public DbSet<DisplayProduct> DisplayProducts { get; init; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>();
            modelBuilder.Entity<DisplayProduct>();

            //modelBuilder.Entity<Customer>();
            //modelBuilder.Entity<SalesOrder>();
            //modelBuilder.Entity<Message>();
        }
    }
}