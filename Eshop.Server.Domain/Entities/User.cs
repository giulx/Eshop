using System.Collections.Generic;
using Eshop.Server.Domain.ValueObjects;
using Eshop.Server.Domain.Events;

namespace Eshop.Server.Domain.Entities
{
    /// Entità User nel dominio.
    /// Rappresenta un customer o un amministratore dell'e-commerce.
    public class User
    {
        // Identità unica
        public int Id { get; private set; }

        // Informazioni anagrafiche
        public string Name { get; private set; } = string.Empty;
        public string Surname { get; private set; } = string.Empty;

        // Email come Value Object
        public Email Email { get; private set; }

        // Sicurezza
        public string PasswordHash { get; private set; } = string.Empty;

        // Ruolo
        public bool IsAdmin { get; private set; }

        // Dati opzionali di contatto/spedizione
        public Address? Address { get; private set; }

        public Cart? Cart { get; private set; }

        // Relazioni
        public List<Order> Orders { get; private set; } = new();

        // --------------------------
        // Costruttori
        // --------------------------

        // Costruttore protetto per EF Core
        protected User() { }


        // Costruttore principale (use case: registrazione o creazione admin)
        public User(string name, string surname, Email email, string passwordHash, bool isAdmin = false)
        {
            Name = name;
            Surname = surname;
            Email = email ?? throw new System.ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new System.ArgumentNullException(nameof(passwordHash));
            IsAdmin = isAdmin;
        }

        // --------------------------
        // Metodi del dominio
        // --------------------------

        /// <summary>
        /// Aggiorna i dati anagrafici dell'user.
        /// </summary>
        public void UpdatePersonalData(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        /// <summary>
        /// Aggiorna i dati di contatto/spedizione.
        /// </summary>
        public void UpdateAddress(Address newAddress)
        {
            Address = newAddress;

            // Qui in futuro potremmo pubblicare un evento di dominio.
            // Esempio:
            // DomainEvents.Publish(new AddressUpdatedEvent(this, newAddress));
        }

        /// <summary>
        /// Aggiorna l'hash della password.
        /// Deve essere generata esternamente dal service applicativo (mai qui).
        /// </summary>
        public void UpdatePasswordHash(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }

        /// <summary>
        /// Cambia il ruolo dell'user (customer/admin).
        /// </summary>
        public void SetAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }
    }
}
