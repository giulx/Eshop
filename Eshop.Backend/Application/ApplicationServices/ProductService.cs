using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Product;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Application.ApplicationServices
{
    /// <summary>
    /// Servizio applicativo per la gestione del catalogo products.
    /// Orquestra i casi d'uso legati ai products (lettura catalogo, creazione, update, delete).
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Restituisce i products come DTO di lettura, con filtro e paginazione.
        /// </summary>
        public async Task<(IReadOnlyList<ProductReadDTO> Items, int TotalCount)> GetAllAsync(
            string? search,
            int page,
            int pageSize)
        {
            // prendo tutto dal repository (se in futuro vuoi ottimizzare, qui metti IQueryable)
            var products = (await _productRepository.GetAllAsync()).ToList();

            // ✅ filtro: mostra solo prodotti attivi (soft delete = ritirati)
            products = products.Where(p => p.IsActive).ToList();

            // filtro
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                products = products
                    .Where(p =>
                        p.Name.ToLower().Contains(lower) ||
                        (p.Description != null && p.Description.ToLower().Contains(lower)))
                    .ToList();
            }

            var total = products.Count;

            // paginazione in memoria
            var skip = (page - 1) * pageSize;
            var pageItems = products
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            // mapping a DTO
            var dtoList = pageItems
                .Select(p => new ProductReadDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price.Value,
                    Currency = p.Price.Currency,
                    AvailableQuantity = p.AvailableQuantity
                })
                .ToList()
                .AsReadOnly();

            return (dtoList, total);
        }

        /// <summary>
        /// Restituisce un singolo product per Id.
        /// </summary>
        public async Task<ProductReadDTO?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            // ✅ se prodotto ritirato, per il pubblico lo trattiamo come non trovato
            if (!product.IsActive)
                return null;

            return new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Value,
                Currency = product.Price.Currency,
                AvailableQuantity = product.AvailableQuantity
            };
        }

        /// <summary>
        /// Create un new product nel catalogo.
        /// </summary>
        public async Task<ProductReadDTO> CreateProductAsync(ProductCreateDTO dto)
        {
            var product = new Product(
                dto.Name,
                dto.Description ?? string.Empty,
                new Money(dto.Price, dto.Currency ?? "EUR"),
                dto.AvailableQuantity
            );

            await _productRepository.AddAsync(product);

            // ritorniamo subito il DTO di lettura del product creato
            return new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.Value,
                Currency = product.Price.Currency,
                AvailableQuantity = product.AvailableQuantity
            };
        }

        /// <summary>
        /// Update le informazioni di un product existing.
        /// Update parziale: aggiorna solo i campi presenti nel DTO.
        /// Ritorna true se aggiornato, false se il product non esiste.
        /// </summary>
        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            // Update description
            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                product.UpdateDescription(dto.Description);
            }

            // ✅ Update name (metodo esiste nel dominio)
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                product.UpdateName(dto.Name);
            }

            // Update price
            if (dto.Price.HasValue)
            {
                var newCurrency = dto.Currency ?? product.Price.Currency;
                product.UpdatePrice(new Money(dto.Price.Value, newCurrency));
            }
            else if (!string.IsNullOrWhiteSpace(dto.Currency))
            {
                // se cambia solo currency e non price: aggiorno il Money mantenendo il valore
                product.UpdatePrice(new Money(product.Price.Value, dto.Currency));
            }

            // Update quantity
            if (dto.AvailableQuantity.HasValue)
            {
                product.UpdateQuantity(dto.AvailableQuantity.Value);
            }

            await _productRepository.UpdateAsync(product);
            return true;
        }

        /// <summary>
        /// Rimuove un product singolo dal catalogo.
        /// Ritorna true se è status trovato ed eliminato, false se non esisteva.
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            await _productRepository.DeleteAsync(id); // ora fa soft delete (Deactivate)
            return true;
        }

        /// <summary>
        /// Rimuove TUTTI i products dal catalogo.
        /// Operazione amministrativa / test.
        /// </summary>
        public async Task DeleteAllAsync()
        {
            await _productRepository.DeleteAllAsync(); // ora fa soft delete in blocco
        }
    }
}
