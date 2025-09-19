using GestionPedidos.Models.Domain;

namespace GestionPedidos.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalSales { get; set; }
        public IEnumerable<Order> RecentOrders { get; set; } = new List<Order>();
        public IEnumerable<Product> LowStockProducts { get; set; } = new List<Product>();
        public User CurrentUser { get; set; } = null!;
    }
}