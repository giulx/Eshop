using Eshop.Server.Domain.Modelli;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Server.Infrastructure.Persistenza
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
        public DbSet<Utente> Utenti => Set<Utente>();
        public DbSet<Prodotto> Prodotti => Set<Prodotto>();
        public DbSet<Ordine> Ordini => Set<Ordine>();
        public DbSet<VoceOrdine> VociOrdine => Set<VoceOrdine>();
        public DbSet<Carrello> Carrelli => Set<Carrello>();
        public DbSet<VoceCarrello> VociCarrello => Set<VoceCarrello>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================
            // UTENTE
            // ======================
            modelBuilder.Entity<Utente>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Cognome)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.IsAdmin)
                      .HasDefaultValue(false);

                // Email VO
                entity.OwnsOne(u => u.Email, email =>
                {
                    email.Property(e => e.Valore)
                         .HasColumnName("Email")
                         .IsRequired()
                         .HasMaxLength(256);

                    // se EF ti dà noia qui, puoi spostare l'indice fuori
                    email.HasIndex(e => e.Valore).IsUnique();
                });

                // Indirizzo VO (opzionale)
                entity.OwnsOne(u => u.Indirizzo, indirizzo =>
                {
                    indirizzo.Property(i => i.Via)
                             .HasColumnName("Via")
                             .HasMaxLength(200);
                    indirizzo.Property(i => i.Citta)
                             .HasColumnName("Citta")
                             .HasMaxLength(100);
                    indirizzo.Property(i => i.CAP)
                             .HasColumnName("CAP")
                             .HasMaxLength(20);
                    indirizzo.Property(i => i.Telefono)
                             .HasColumnName("Telefono")
                             .HasMaxLength(30);
                });

                // 1 Utente -> 1 Carrello
                entity.HasOne(u => u.Carrello)
                      .WithOne(c => c.Cliente)
                      .HasForeignKey<Carrello>(c => c.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 1 Utente -> molti Ordini
                entity.HasMany(u => u.Ordini)
                      .WithOne(o => o.Cliente)
                      .HasForeignKey(o => o.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // PRODOTTO
            // ======================
            modelBuilder.Entity<Prodotto>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Nome)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(p => p.Descrizione)
                      .HasMaxLength(500);

                entity.Property(p => p.QuantitaDisponibile)
                      .IsRequired();

                entity.OwnsOne(p => p.Prezzo, money =>
                {
                    money.Property(m => m.Valore)
                         .HasColumnName("Prezzo")
                         .HasPrecision(18, 2)
                         .IsRequired();

                    money.Property(m => m.Valuta)
                         .HasColumnName("Valuta")
                         .IsRequired()
                         .HasMaxLength(3);
                });
            });

            // ======================
            // ORDINE
            // ======================
            modelBuilder.Entity<Ordine>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.DataCreazione)
                      .IsRequired();

                // enum salvato come stringa
                entity.Property(o => o.Stato)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .IsRequired();

                entity.HasMany(o => o.Voci)
                      .WithOne(v => v.Ordine)
                      .HasForeignKey(v => v.OrdineId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // VOCE ORDINE
            // ======================
            modelBuilder.Entity<VoceOrdine>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.ProdottoId)
                      .IsRequired();

                entity.Property(v => v.NomeProdotto)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(v => v.Quantita)
                      .IsRequired();

                // PrezzoUnitario come VO
                entity.OwnsOne(v => v.PrezzoUnitario, money =>
                {
                    money.Property(m => m.Valore)
                         .HasColumnName("PrezzoUnitario")
                         .HasPrecision(18, 2)
                         .IsRequired();

                    money.Property(m => m.Valuta)
                         .HasColumnName("Valuta")
                         .IsRequired()
                         .HasMaxLength(3);
                });

                // 👇 questa è la riga che risolve il tuo errore
                // Subtotale nella tua entità è solo get e EF non può mapparla
                entity.Ignore(v => v.Subtotale);
            });

            // ======================
            // CARRELLO
            // ======================
            modelBuilder.Entity<Carrello>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.UltimoAggiornamentoPrezziUtc)
                      .IsRequired();

                entity.HasMany(c => c.Voci)
                      .WithOne(v => v.Carrello)
                      .HasForeignKey(v => v.CarrelloId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // VOCE CARRELLO
            // ======================
            modelBuilder.Entity<VoceCarrello>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.HasOne(v => v.Prodotto)
                      .WithMany()
                      .HasForeignKey(v => v.ProdottoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(v => new { v.CarrelloId, v.ProdottoId })
                      .IsUnique();

                entity.OwnsOne(v => v.Quantita, q =>
                {
                    q.Property(qv => qv.Valore)
                     .HasColumnName("Quantita")
                     .IsRequired();
                });

                entity.OwnsOne(v => v.PrezzoUnitarioSnapshot, t =>
                {
                    t.Property(m => m.Valore)
                     .HasColumnName("PrezzoSnapshot")
                     .HasPrecision(18, 2)
                     .IsRequired();

                    t.Property(m => m.Valuta)
                     .HasColumnName("ValutaSnapshot")
                     .IsRequired()
                     .HasMaxLength(3);
                });
            });
        }
    }
}
