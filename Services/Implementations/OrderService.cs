using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using GestionPedidos.Services.Interfaces;

namespace GestionPedidos.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(int customerId, List<(int ProductId, int Quantity)> items, string notes = "")
        {
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                // Validate stock availability
                if (!await ValidateStockAvailabilityAsync(items))
                {
                    throw new InvalidOperationException("Stock insuficiente para uno o más productos");
                }

                // Create order
                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Notes = notes
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Create order items and calculate total
                decimal total = 0;
                foreach (var (productId, quantity) in items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(productId);
                    if (product == null) continue;

                    var subtotal = product.Price * quantity;
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        Subtotal = subtotal
                    };

                    order.OrderItems.Add(orderItem);
                    total += subtotal;

                    // Reduce stock
                    await _unitOfWork.Products.ReduceStockAsync(productId, quantity);
                }

                order.Total = total;
                _unitOfWork.Orders.Update(order);
                
                await _unitOfWork.CommitTransactionAsync();
                return order;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) return false;

                order.Status = status;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _unitOfWork.Orders.GetWithItemsAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId)
        {
            return await _unitOfWork.Orders.GetByCustomerIdAsync(customerId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _unitOfWork.Orders.GetByStatusAsync(status);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            return await UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<(int ProductId, int Quantity)> items)
        {
            decimal total = 0;
            foreach (var (productId, quantity) in items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product != null)
                {
                    total += product.Price * quantity;
                }
            }
            return total;
        }

        public async Task<bool> ValidateStockAvailabilityAsync(List<(int ProductId, int Quantity)> items)
        {
            foreach (var (productId, quantity) in items)
            {
                if (!await _unitOfWork.Products.HasSufficientStockAsync(productId, quantity))
                {
                    return false;
                }
            }
            return true;
        }
    }
}