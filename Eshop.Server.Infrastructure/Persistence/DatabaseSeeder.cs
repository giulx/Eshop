using System;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eshop.Server.Infrastructure.Persistence
{
    public class DatabaseSeeder
    {
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<string>>();

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
