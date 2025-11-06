using System.Collections.Generic;
using ApiServer.Domain.ValueObjects;
using ApiServer.Domain.Events;

namespace ApiServer.Domain.Models
{
    /// <summary>
    /// Entità Utente nel dominio.
    /// Rappresenta un cliente o un amministratore dell'e-commerce.
    /// </summary>
    public class Utente
    {
        // Identità unica
        public int Id { get; private set; }

        // Informazioni anagrafiche
        public string Nome { get; private set; } = string.Empty;
        public string Cognome { get; private set; } = string.Empty;

        // Email immutabile
        public Email Email { get; private set; }

        // Sicurezza
        public string PasswordHash { get; private set; } = string.Empty;

        // Ruolo
        public bool IsAdmin { get; private set; }

        // Dati opzionali di contatto/spedizione
        public Indirizzo? Indirizzo { get; private set; }

        // Relazioni
        public List<Ordine> Ordini { get; private set; } = new();

        // --------------------------
        // Costruttori
        // --------------------------

        // EF / ORM friendly
        protected Utente() { }

        // Costruttore principale
        public Utente(string nome, string cognome, Email email, string passwordHash, bool isAdmin = false)
        {
            Nome = nome;
            Cognome = cognome;
            Email = email ?? throw new System.ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new System.ArgumentNullException(nameof(passwordHash));
            IsAdmin = isAdmin;
        }

        // --------------------------
        // Metodi del dominio
        // --------------------------

        /// <summary>
        /// Aggiorna i dati anagrafici dell'utente.
        /// </summary>
        public void AggiornaDatiPersonali(string nome, string cognome)
        {
            Nome = nome;
            Cognome = cognome;
        }

        /// <summary>
        /// Aggiorna i dati di contatto/spedizione.
        /// </summary>
        public void AggiornaIndirizzo(Indirizzo nuovoIndirizzo)
        {
            Indirizzo = nuovoIndirizzo;

            // Esempio di evento di dominio
            // DomainEvents.Publish(new IndirizzoAggiornato(this, nuovoIndirizzo));
        }

        /// <summary>
        /// Aggiorna l'hash della password.
        /// Deve essere generata esternamente dal service.
        /// </summary>
        public void AggiornaPasswordHash(string nuovaPasswordHash)
        {
            PasswordHash = nuovaPasswordHash;
        }

        /// <summary>
        /// Cambia il ruolo dell'utente.
        /// </summary>
        public void ImpostaAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }
    }
}
