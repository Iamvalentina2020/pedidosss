using GestionPedidos.Models.Domain;

namespace GestionPedidos.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
        Task<Order?> GetWithItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<decimal> GetTotalSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}