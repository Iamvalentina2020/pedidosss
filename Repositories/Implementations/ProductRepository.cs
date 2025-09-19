using GestionPedidos.Data;
using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPedidos.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(name) && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(p => p.Category == category && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> HasSufficientStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            return product != null && product.Stock >= quantity;
        }

        public async Task ReduceStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null && product.Stock >= quantity)
            {
                product.Stock -= quantity;
                Update(product);
            }
            else
            {
                throw new InvalidOperationException("Stock insuficiente para el producto");
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }
    }
}