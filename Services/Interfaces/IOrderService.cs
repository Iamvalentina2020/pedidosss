using GestionPedidos.Models.Domain;

namespace GestionPedidos.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(int customerId, List<(int ProductId, int Quantity)> items, string notes = "");
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> CancelOrderAsync(int orderId);
        Task<decimal> CalculateOrderTotalAsync(List<(int ProductId, int Quantity)> items);
        Task<bool> ValidateStockAvailabilityAsync(List<(int ProductId, int Quantity)> items);
    }
}