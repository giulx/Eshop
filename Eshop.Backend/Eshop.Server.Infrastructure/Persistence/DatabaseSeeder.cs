using System;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector; // <-- aggiunto SOLO per catturare l'eccezione MySQL

namespace Eshop.Server.Infrastructure.Persistence
{
    public class DatabaseSeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<string>>();

            // ✅ Fix necessario: MySQL in Docker potrebbe non essere pronto al primo avvio.
            // Facciamo retry prima di migrazioni/seed.
            const int maxAttempts = 30;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    // 1️⃣ Applica le migration (solo per sicurezza)
                    await context.Database.MigrateAsync();

                    // 2️⃣ Se non ci sono utenti → creali
                    if (!await context.Users.AnyAsync())
                    {
                        var commonPassword = "password123";

                        var users = new[]
                        {
                            new UserData("Admin", "Super", "admin1@eshop.com", true, "Via Roma", "Ferrara", "44121", "12"),
                            new UserData("Admin", "Master", "admin2@eshop.com", true, "Corso Garibaldi", "Ferrara", "44122", "45"),

                            new UserData("John", "Doe", "customer1@eshop.com", false, "Via Po", "Ferrara", "44123", "8"),
                            new UserData("Jane", "Doe", "customer2@eshop.com", false, "Via Savonarola", "Ferrara", "44124", "22"),
                            new UserData("Mario", "Rossi", "customer3@eshop.com", false, "Via Bologna", "Ferrara", "44125", "35"),
                            new UserData("Luisa", "Bianchi", "customer4@eshop.com", false, "Via Darsena", "Ferrara", "44126", "6")
                        };

                        foreach (var u in users)
                        {
                            var hash = passwordHasher.HashPassword(u.Email, commonPassword);

                            var newUser = new User(
                                u.Name,
                                u.Surname,
                                new Email(u.Email),
                                hash,
                                u.IsAdmin
                            );

                            var address = new Address(
                                u.Street,
                                u.City,
                                u.PostalCode,
                                u.Number
                            );

                            newUser.UpdateAddress(address);

                            context.Users.Add(newUser);
                        }

                        await context.SaveChangesAsync();
                    }

                    // ✅ 2.5️⃣ Se non ci sono prodotti → creane ~50
                    if (!await context.Products.AnyAsync())
                    {
                        var rnd = new Random(12345);

                        var catalog = new (string Name, string Desc)[]
                        {
                            ("Smartphone Aurora X", "Smartphone 6.5\" OLED, 128GB, dual SIM."),
                            ("Laptop Nebula 14", "Notebook 14\" leggero, ottimo per studio e lavoro."),
                            ("Cuffie EchoPods", "Cuffie wireless con cancellazione del rumore."),
                            ("Mouse ProGrip", "Mouse ergonomico con DPI regolabili."),
                            ("Tastiera MechaLite", "Tastiera meccanica compatta per gaming e coding."),
                            ("Monitor Crystal 27", "Monitor 27\" QHD, 144Hz."),
                            ("Smartwatch Pulse 2", "Smartwatch con cardio, sonno, GPS."),
                            ("Speaker BoomMini", "Cassa Bluetooth portatile con bassi potenti."),
                            ("Webcam ClearView", "Webcam 1080p per meeting e streaming."),
                            ("SSD Rapid 1TB", "SSD NVMe 1TB ad alte prestazioni."),
                            ("PowerBank Titan 20K", "Batteria esterna 20000mAh USB-C PD."),
                            ("Router AirMesh", "Wi-Fi veloce e stabile per casa/ufficio."),
                            ("Stampante InkJet Home", "Stampante compatta per uso domestico."),
                            ("Zaino UrbanTech", "Zaino resistente con vano laptop 15\"."),
                            ("Borraccia SteelPro", "Borraccia termica in acciaio 750ml."),
                            ("Lampada DeskGlow", "Lampada da scrivania LED dimmerabile."),
                            ("Sedia ErgoComfort", "Sedia ergonomica con supporto lombare."),
                            ("Cavo USB-C Max", "Cavo USB-C robusto, ricarica rapida."),
                            ("Hub USB MultiPort", "Hub USB con HDMI e lettore SD."),
                            ("Microfono StreamMic", "Microfono USB per podcast/streaming.")
                        };

                        for (int i = 1; i <= 50; i++)
                        {
                            var t = catalog[rnd.Next(catalog.Length)];
                            var name = $"{t.Name} #{i}";
                            var desc = t.Desc;

                            var priceValue = Math.Round((decimal)(rnd.NextDouble() * 990.0 + 9.99), 2);
                            var qty = rnd.Next(0, 201);

                            // ⚠️ Se qui NON compila, è perché il tuo Money ha un costruttore diverso.
                            // In quel caso incollami la classe Money e lo adatto.
                            var money = new Money(priceValue, "EUR");

                            var product = new Product(name, desc, money, qty);
                            context.Products.Add(product);
                        }

                        await context.SaveChangesAsync();
                    }

                    // 3️⃣ Crea carrello per tutti i non admin che non lo hanno
                    var customersWithoutCart = await context.Users
                        .Include(u => u.Cart)
                        .Where(u => !u.IsAdmin && u.Cart == null)
                        .ToListAsync();

                    foreach (var customer in customersWithoutCart)
                    {
                        var cart = (Cart)Activator.CreateInstance(typeof(Cart), nonPublic: true)!;

                        context.Carts.Add(cart);
                        context.Entry(cart).Property("LastPriceUpdateUtc").CurrentValue = DateTime.UtcNow;
                        context.Entry(cart).Property("CustomerId").CurrentValue = customer.Id;
                    }

                    await context.SaveChangesAsync();

                    // ✅ Se arriviamo qui, tutto ok → usciamo dal retry.
                    return;
                }
                catch (MySqlException ex)
                {
                    // MySQL non pronto / connessione rifiutata: aspetta e riprova.
                    Console.WriteLine($"Seeder: MySQL non pronto (tentativo {attempt}/{maxAttempts}): {ex.Message}");

                    if (attempt == maxAttempts)
                        throw;

                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }

        /// <summary>
        /// Struttura dati interna solo per organizzare gli utenti iniziali.
        /// </summary>
        private record UserData(
            string Name,
            string Surname,
            string Email,
            bool IsAdmin,
            string Street,
            string City,
            string PostalCode,
            string Number
        );
    }
}
