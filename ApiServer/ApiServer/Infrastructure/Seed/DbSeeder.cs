using ApiServer.Domain.Models;
using ApiServer.Domain.ValueObjects;
using ApiServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Facades
{
    public static class DbSeeder
    {
        public static void Seed(ApiServerContext db)
        {
            // Applica eventuali migrazioni pendenti
            db.Database.Migrate();

            // =========================
            // Utenti
            // =========================
            if (!db.Utenti.Any())
            {
                var admin = new Utente(
                    nome: "Admin",
                    cognome: "Super",
                    email: new Email("admin@api.com"),
                    passwordHash: "hash_admin", // sostituire con hash reale
                    isAdmin: true
                );

                var cliente = new Utente(
                    nome: "Mario",
                    cognome: "Rossi",
                    email: new Email("mario.rossi@api.com"),
                    passwordHash: "hash_cliente" // sostituire con hash reale
                );

                db.Utenti.AddRange(admin, cliente);
                db.SaveChanges();
            }

            // =========================
            // Prodotti
            // =========================
            if (!db.Prodotti.Any())
            {
                var prodotti = new List<Prodotto>
                {
                    new Prodotto("Laptop", "Laptop potente",  new Money(1200)),
                    new Prodotto("Mouse", "Mouse ergonomico", new Money(35)),
                    new Prodotto("Tastiera", "Tastiera meccanica", new Money(80))
                };

                db.Prodotti.AddRange(prodotti);
                db.SaveChanges();
            }

            // =========================
            // Ordini
            // =========================
            if (!db.Ordini.Any())
            {
                var cliente = db.Utenti.FirstOrDefault(u => u.Email.Address == "mario.rossi@api.com");
                var laptop = db.Prodotti.FirstOrDefault(p => p.Nome == "Laptop");
                var mouse = db.Prodotti.FirstOrDefault(p => p.Nome == "Mouse");

                if (cliente != null && laptop != null && mouse != null)
                {
                    var ordine = new Ordine(cliente.Id);
                    ordine.AggiungiVoce(new VoceOrdine(laptop.Id, laptop.Nome, laptop.Prezzo, 1));
                    ordine.AggiungiVoce(new VoceOrdine(mouse.Id, mouse.Nome, mouse.Prezzo, 2));

                    db.Ordini.Add(ordine);
                    db.SaveChanges();
                }
            }
        }
    }
}
