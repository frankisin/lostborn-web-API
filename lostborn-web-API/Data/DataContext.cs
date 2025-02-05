using System.Text.Json;
using lostborn_backend.Models;
using Microsoft.EntityFrameworkCore;


namespace lostborn_backend.Helpers
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{

		}
        public DbSet<IPAccess> Access { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem>InvoiceItems { get; set; }
        public DbSet<Transaction>Transactions { get; set; }
        public DbSet<PaymentMethod>PaymentMethods { get; set; }
        public DbSet<ShippingAddress>ShippingAddresses { get; set; }
        public DbSet<CasinoTransaction> CasinoTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-One relationship between Users and Carts
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Cart)          // One User has one Cart
                .WithOne(c => c.User)         // One Cart belongs to one User
                .HasForeignKey<Carts>(c => c.UserID); // Foreign key in Carts referencing Users
                               

            // One-to-Many relationship between Carts and CartItems
            modelBuilder.Entity<Carts>()
                .HasMany(c => c.CartItems)    // One Cart has many CartItems
                .WithOne(ci => ci.Cart)       // One CartItem belongs to one Cart
                .HasForeignKey(ci => ci.CartID) // Foreign key in CartItems referencing Carts
                .IsRequired();

            // Add other configurations if needed

            base.OnModelCreating(modelBuilder);
        }

        internal Task SaveChangesAsync(JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }	

   
}

