using Eshop.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Server.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ========================
        // DbSet
        // ========================
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================
            // UTENTE
            // ======================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Surname)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.IsAdmin)
                      .HasDefaultValue(false);

                // Email VO (Owned)
                entity.OwnsOne(u => u.Email, email =>
                {
                    email.Property(e => e.Value)
                         .HasColumnName("Email")
                         .IsRequired()
                         .HasMaxLength(256);

                    // ✅ UNIQUE index sulla colonna Email (Users.Email)
                    email.HasIndex(e => e.Value).IsUnique();
                });

                // Address VO (opzionale)
                entity.OwnsOne(u => u.Address, address =>
                {
                    address.Property(i => i.Street)
                           .HasColumnName("Street")
                           .HasMaxLength(200);

                    address.Property(i => i.City)
                           .HasColumnName("City")
                           .HasMaxLength(100);

                    address.Property(i => i.PostalCode)
                           .HasColumnName("PostalCode")
                           .HasMaxLength(20);

                    address.Property(i => i.Number)
                           .HasColumnName("Number")
                           .HasMaxLength(30);
                });

                // 1 User -> 1 Cart
                entity.HasOne(u => u.Cart)
                      .WithOne(c => c.Customer)
                      .HasForeignKey<Cart>(c => c.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 1 User -> molti Orders
                entity.HasMany(u => u.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // PRODOTTO
            // ======================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(p => p.Description)
                      .HasMaxLength(500);

                entity.Property(p => p.AvailableQuantity)
                      .IsRequired();

                // ✅ soft delete / ritiro prodotto
                entity.Property(p => p.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);

                entity.OwnsOne(p => p.Price, money =>
                {
                    money.Property(m => m.Value)
                         .HasColumnName("Price")
                         .HasPrecision(18, 2)
                         .IsRequired();

                    money.Property(m => m.Currency)
                         .HasColumnName("Currency")
                         .IsRequired()
                         .HasMaxLength(3);
                });
            });

            // ======================
            // ORDINE
            // ======================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.CreationDate)
                      .IsRequired();

                // enum salvato come stringa
                entity.Property(o => o.Status)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .IsRequired();

                entity.HasMany(o => o.Items)
                      .WithOne(v => v.Order)
                      .HasForeignKey(v => v.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // VOCE ORDINE
            // ======================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.ProductId)
                      .IsRequired();

                entity.Property(v => v.ProductName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(v => v.Quantity)
                      .IsRequired();

                // UnitPrice come VO
                entity.OwnsOne(v => v.UnitPrice, money =>
                {
                    money.Property(m => m.Value)
                         .HasColumnName("UnitPrice")
                         .HasPrecision(18, 2)
                         .IsRequired();

                    money.Property(m => m.Currency)
                         .HasColumnName("Currency")
                         .IsRequired()
                         .HasMaxLength(3);
                });

                entity.Ignore(v => v.Subtotal);
            });

            // ======================
            // CARRELLO
            // ======================
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.LastPriceUpdateUtc)
                      .IsRequired();

                entity.HasMany(c => c.Items)
                      .WithOne(v => v.Cart)
                      .HasForeignKey(v => v.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // VOCE CARRELLO
            // ======================
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.HasOne(v => v.Product)
                      .WithMany()
                      .HasForeignKey(v => v.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => new { v.CartId, v.ProductId })
                      .IsUnique();

                entity.OwnsOne(v => v.Quantity, q =>
                {
                    q.Property(qv => qv.Value)
                     .HasColumnName("Quantity")
                     .IsRequired();
                });

                entity.OwnsOne(v => v.UnitPriceSnapshot, t =>
                {
                    t.Property(m => m.Value)
                     .HasColumnName("PriceSnapshot")
                     .HasPrecision(18, 2)
                     .IsRequired();

                    t.Property(m => m.Currency)
                     .HasColumnName("CurrencySnapshot")
                     .IsRequired()
                     .HasMaxLength(3);
                });
            });
        }
    }
}
