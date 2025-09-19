using GestionPedidos.Models.Domain;

namespace GestionPedidos.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> HasSufficientStockAsync(int productId, int quantity);
        Task ReduceStockAsync(int productId, int quantity);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}