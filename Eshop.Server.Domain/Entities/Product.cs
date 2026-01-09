using System;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Domain.Entities
{
    // Entità del dominio che rappresenta un product vendibile nel catalogo.
    public class Product
    {
        public int Id { get; private set; }

        // Name del product.
        public string Name { get; private set; } = string.Empty;

        // Description testuale del product.
        public string Description { get; private set; } = string.Empty;

        // Price unitario espresso come Value Object.
        public Money Price { get; private set; }

        // Quantity disponibile in magazzino.
        public int AvailableQuantity { get; private set; }

        // Indica se il prodotto è attivo (visibile/acquistabile) nel catalogo.
        // Default: true per i nuovi prodotti.
        public bool IsActive { get; private set; } = true;

        // Costruttore richiesto da Entity Framework
        protected Product() { }

        // Crea un new product con i dati principali
        public Product(string name, string description, Money price, int availableQuantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il name del product non può essere vuoto.", nameof(name));

            Price = price ?? throw new ArgumentNullException(nameof(price));

            if (availableQuantity < 0)
                throw new ArgumentException("La quantity disponibile non può essere negativa.", nameof(availableQuantity));

            Name = name;
            Description = description ?? string.Empty;
            AvailableQuantity = availableQuantity;

            // nuovo prodotto: attivo di default
            IsActive = true;
        }

        // -------------------------
        // aggiornamenti "classici"
        // -------------------------
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("La quantity non può essere negativa.", nameof(newQuantity));

            AvailableQuantity = newQuantity;
        }

        public void UpdatePrice(Money newPrice)
        {
            Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        }

        public void UpdateDescription(string newDescription)
        {
            if (newDescription == null)
                throw new ArgumentNullException(nameof(newDescription));

            Description = newDescription.Trim();
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Il name non può essere vuoto.", nameof(newName));

            Name = newName.Trim();
        }

        // -------------------------
        // lifecycle (ritiro prodotto)
        // -------------------------
        /// <summary>
        /// Ritira il prodotto dal catalogo: non viene eliminato dal DB,
        /// ma non deve più essere vendibile/visibile. Imposta anche la quantity a 0.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            AvailableQuantity = 0;
        }

        // -------------------------
        // metodi per ORDINI
        // -------------------------

        /// <summary>
        /// True se il magazzino ha almeno la quantity richiesta.
        /// </summary>
        public bool CheckAvailability(int requiredQuantity)
        {
            if (requiredQuantity <= 0)
                return false;

            // se il prodotto è disattivato, non è disponibile
            if (!IsActive)
                return false;

            return AvailableQuantity >= requiredQuantity;
        }

        /// <summary>
        /// Scala la quantity dal magazzino.
        /// Da chiamare SOLO quando l’order è confermato.
        /// </summary>
        public void DecreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La quantity da scalare dev'essere positiva.", nameof(quantity));

            if (!IsActive)
                throw new InvalidOperationException("Prodotto non attivo.");

            if (quantity > AvailableQuantity)
                throw new InvalidOperationException("Quantity richiesta superiore alla disponibilità.");

            AvailableQuantity -= quantity;
        }
    }
}
