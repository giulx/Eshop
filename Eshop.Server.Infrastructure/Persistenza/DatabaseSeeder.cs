using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eshop.Server.Infrastructure.Persistenza
{
    public class DatabaseSeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<string>>();

            // Assicuriamoci che le migration siano applicate
            await context.Database.MigrateAsync();

            //
            // 1) PRODOTTI
            //
            if (!await context.Prodotti.AnyAsync())
            {
                context.Prodotti.AddRange(
                    // Abbigliamento
                    new Prodotto("Maglietta con logo", "Maglietta con il logo dell'azienda, 100% cotone.", new Money(19.99m, "EUR"), 50),
                    new Prodotto("Felpa sportiva", "Felpa in cotone per allenamenti o tempo libero.", new Money(29.99m, "EUR"), 70),
                    new Prodotto("Pantaloni jeans", "Jeans taglio slim per un look casual.", new Money(49.99m, "EUR"), 40),
                    new Prodotto("Giacca di pelle", "Giacca in vera pelle per look elegante.", new Money(129.99m, "EUR"), 30),
                    new Prodotto("Cappello estate", "Cappello in paglia per l'estate.", new Money(15.99m, "EUR"), 60),
                    new Prodotto("Sciarpa in lana", "Sciarpa in lana per l'inverno.", new Money(19.99m, "EUR"), 50),

                    // Elettronica
                    new Prodotto("Smartphone 5G", "Smartphone 5G con fotocamera avanzata e display OLED.", new Money(799.99m, "EUR"), 25),
                    new Prodotto("Laptop Gaming", "Laptop da gaming con GPU potente.", new Money(1299.99m, "EUR"), 10),
                    new Prodotto("Cuffie wireless", "Cuffie Bluetooth con cancellazione del rumore.", new Money(89.99m, "EUR"), 60),
                    new Prodotto("Orologio Smart", "Orologio smart con monitoraggio fitness.", new Money(199.99m, "EUR"), 50),
                    new Prodotto("Televisore 4K", "Televisore 4K LED Ultra HD, schermo 55 pollici.", new Money(599.99m, "EUR"), 15),
                    new Prodotto("Smartwatch per fitness", "Smartwatch per il monitoraggio dell'attività fisica.", new Money(99.99m, "EUR"), 40),

                    // Casa
                    new Prodotto("Tavolo da ufficio", "Tavolo moderno per ufficio, in legno massello.", new Money(249.99m, "EUR"), 18),
                    new Prodotto("Sedia ergonomica", "Sedia ergonomica con supporto lombare per lunghi periodi.", new Money(129.99m, "EUR"), 50),
                    new Prodotto("Lettino da giardino", "Lettino reclinabile per il giardino in metallo e tessuto.", new Money(89.99m, "EUR"), 30),
                    new Prodotto("Lampada LED da scrivania", "Lampada da scrivania con luce regolabile.", new Money(29.99m, "EUR"), 100),
                    new Prodotto("Cuscini decorativi", "Cuscini decorativi per divani, set da 2.", new Money(19.99m, "EUR"), 70),
                    new Prodotto("Scaffale in legno", "Scaffale in legno naturale per libreria.", new Money(89.99m, "EUR"), 40),

                    // Sport
                    new Prodotto("Pallone da calcio", "Pallone ufficiale per calcio, ideale per allenamenti.", new Money(29.99m, "EUR"), 100),
                    new Prodotto("Tapis roulant", "Tapis roulant elettrico per allenamenti a casa.", new Money(499.99m, "EUR"), 20),
                    new Prodotto("Bicicletta da corsa", "Bicicletta da corsa leggera e resistente.", new Money(899.99m, "EUR"), 15),
                    new Prodotto("Tenda da campeggio", "Tenda da campeggio con 2 posti, impermeabile.", new Money(129.99m, "EUR"), 35),
                    new Prodotto("Zaino da trekking", "Zaino da trekking per escursioni, 50 litri.", new Money(79.99m, "EUR"), 60),
                    new Prodotto("Guanti da palestra", "Guanti da palestra per il fitness e bodybuilding.", new Money(15.99m, "EUR"), 80),

                    // Accessori
                    new Prodotto("Cintura in pelle", "Cintura elegante in pelle nera.", new Money(39.99m, "EUR"), 45),
                    new Prodotto("Portafoglio in pelle", "Portafoglio in pelle, design compatto.", new Money(29.99m, "EUR"), 50),
                    new Prodotto("Zaino da viaggio", "Zaino da viaggio in nylon resistente con scomparti.", new Money(59.99m, "EUR"), 40),
                    new Prodotto("Valigetta da ufficio", "Valigetta elegante in pelle per documenti.", new Money(149.99m, "EUR"), 25),
                    new Prodotto("Borsa a tracolla", "Borsa a tracolla in tessuto per uomo e donna.", new Money(49.99m, "EUR"), 60),
                    new Prodotto("Cuffie da gaming", "Cuffie da gaming con microfono incorporato.", new Money(69.99m, "EUR"), 50)
                );

                await context.SaveChangesAsync();
            }

            // 2) UTENTI
            var commonPassword = "password123";

            var utentiDaGarantire = new[]
            {
                new {
                    Nome = "Admin", Cognome = "Super",
                    Email = "email1@eshop.com",
                    IsAdmin = true,
                    Indirizzo = new Indirizzo("Via Roma 10", "Ferrara", "44121", "3401111111")
                },
                new {
                    Nome = "Admin", Cognome = "Master",
                    Email = "email2@eshop.com",
                    IsAdmin = true,
                    Indirizzo = new Indirizzo("Via Garibaldi 5", "Ferrara", "44122", "3402222222")
                },
                new {
                    Nome = "John", Cognome = "Doe",
                    Email = "email3@eshop.com",
                    IsAdmin = false,
                    Indirizzo = new Indirizzo("Via Po 18", "Ferrara", "44123", "3403333333")
                },
                new {
                    Nome = "Jane", Cognome = "Doe",
                    Email = "email4@eshop.com",
                    IsAdmin = false,
                    Indirizzo = new Indirizzo("Via Savonarola 22", "Ferrara", "44124", "3404444444")
                },
                new {
                    Nome = "Mario", Cognome = "Rossi",
                    Email = "email5@eshop.com",
                    IsAdmin = false,
                    Indirizzo = new Indirizzo("Via Bologna 90", "Ferrara", "44125", "3405555555")
                },
                new {
                    Nome = "Luisa", Cognome = "Bianchi",
                    Email = "email6@eshop.com",
                    IsAdmin = false,
                    Indirizzo = new Indirizzo("Via Darsena 15", "Ferrara", "44126", "3406666666")
                },
            };

            foreach (var u in utentiDaGarantire)
            {
                var existing = await context.Utenti
                    .FirstOrDefaultAsync(x => x.Email.Valore == u.Email);

                var hash = passwordHasher.HashPassword(u.Email, commonPassword);

                if (existing == null)
                {
                    // Non esiste → crea nuovo con indirizzo
                    var nuovo = new Utente(
                        u.Nome,
                        u.Cognome,
                        new Email(u.Email),
                        hash,
                        u.IsAdmin
                    );

                    nuovo.AggiornaIndirizzo(u.Indirizzo);
                    context.Utenti.Add(nuovo);
                }
                else
                {
                    // Esiste → aggiorna solo i dati base, NON l'indirizzo se già c'è
                    existing.AggiornaDatiPersonali(u.Nome, u.Cognome);
                    existing.ImpostaAdmin(u.IsAdmin);
                    existing.AggiornaPasswordHash(hash);

                    // Aggiorna indirizzo SOLO se l'utente NON ne ha ancora uno
                    if (existing.Indirizzo == null)
                    {
                        existing.AggiornaIndirizzo(u.Indirizzo);
                    }
                }
            }

            await context.SaveChangesAsync();

            // 3) CARRELLI per utenti non admin senza carrello
            var utentiSenzaCarrello = await context.Utenti
                .Include(u => u.Carrello)
                .Where(u => !u.IsAdmin && u.Carrello == null)
                .ToListAsync();

            foreach (var u in utentiSenzaCarrello)
            {
                // crea istanza di Carrello anche se ha costruttore non pubblico
                var carrello = (Carrello)Activator.CreateInstance(typeof(Carrello), nonPublic: true)!;

                context.Carrelli.Add(carrello);

                context.Entry(carrello).Property("UltimoAggiornamentoPrezziUtc").CurrentValue = DateTime.UtcNow;
                context.Entry(carrello).Property("ClienteId").CurrentValue = u.Id;
            }

            await context.SaveChangesAsync();
        }
    }
}
