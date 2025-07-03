
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> users { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Pharmacy> pharmacies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ChatBotHistory> ChatBotHistory { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductPharmacy> ProductPharmacies { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ChatBotHistory to Conversations: One-to-Many relationship
            modelBuilder.Entity<ChatBotHistory>()
                .HasMany(ch => ch.Conversations)
                .WithOne(c => c.ChatHistory)
                .HasForeignKey(c => c.ChatHistoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Conversation to Message: One-to-Many relationship
            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<CartItem>()
        .Property(c => c.Id)
        .ValueGeneratedOnAdd();


            modelBuilder.Entity<Order>()
    .HasOne(o => o.payment)
    .WithOne(p => p.Order)
    .HasForeignKey<Payment>(p => p.OrderID);
          
            modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderID, oi.ProductID });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductID);

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartID, ci.ProductID });

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartID);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductID);

            modelBuilder.Entity<ProductPharmacy>()
                .HasKey(pp => new { pp.ProductID, pp.PharmacyID });

            modelBuilder.Entity<ProductPharmacy>()
                .HasOne(pp => pp.pharmacy)
                .WithMany(ph => ph.productPharmacy)  // Ensure this property exists in Pharmacy class
                .HasForeignKey(pp => pp.PharmacyID);

            modelBuilder.Entity<ProductPharmacy>()
                .HasOne(pp => pp.product)
                .WithMany(p => p.productPharmacy)  // Ensure this property exists in Product class
                .HasForeignKey(pp => pp.ProductID);

            base.OnModelCreating(modelBuilder); // Move this to the last line

            /*  string staticAdminPasswordHash = "$2a$10$LJxw7HfZOrYxgFsVgV59CeAG / sD1Gerz1f8gWwG25meE9tcV5sTF";

              modelBuilder.Entity<User>().HasData(new User
              {
                  UserID = 1, // Fixed ID for Admin
                  UserName = "Admin",
                  Password = staticAdminPasswordHash, // Use the fixed hash
                //  FirstName = "System",
                //  LastName = "Administrator",
                //  Phone = "0000000000",
               //  Email = "admin@example.com",
                  Role = "Admin"
              });*/
            //  string adminPassword = "123A456D789M@admin";
            string adminPasswordHash = "$2a$10$gxPoIhi3KkMKlc5Cexy0g.PBcU6PBFwpqqGPiYLSo9AfgxE2V6hqC";

            modelBuilder.Entity<User>().HasData(new User
            {
                UserID = 1,
                UserName = "admin",
                Password = adminPasswordHash,
                Email="",
                Role = "Admin",
                FirstName="",
                LastName="",
                Phone=""
                // No other personal data fields
            });
        }




    }
}
