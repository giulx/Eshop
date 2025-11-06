using ApiServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        // Costruttore
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ========================
        // DbSet per tutte le entità
        // ========================
        public DbSet<Utente> Utenti { get; set; } = null!;
        public DbSet<Prodotto> Prodotti { get; set; } = null!;
        public DbSet<Ordine> Ordini { get; set; } = null!;
        public DbSet<VoceOrdine> VociOrdine { get; set; } = null!;

        // ========================
        // Configurazioni Fluent API
        // ========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Utente
            modelBuilder.Entity<Utente>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique(); // email univoca
                entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Cognome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
            });

            // Prodotto
            modelBuilder.Entity<Prodotto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Descrizione).HasMaxLength(500);
                entity.OwnsOne(p => p.Prezzo); // Money come ValueObject
            });

            // Ordine
            modelBuilder.Entity<Ordine>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.HasOne(o => o.Cliente)
                      .WithMany(u => u.Ordini)
                      .HasForeignKey(o => o.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // VoceOrdine
            modelBuilder.Entity<VoceOrdine>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasOne(v => v.Ordine)
                      .WithMany(o => o.Voci)
                      .HasForeignKey(v => v.OrdineId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.OwnsOne(v => v.Prezzo); // Money come ValueObject
            });
        }
    }
}
