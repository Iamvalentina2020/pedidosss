using GestionPedidos.Controllers;
using GestionPedidos.Filters;
using GestionPedidos.Models.Domain;
using GestionPedidos.Models.ViewModels;
using GestionPedidos.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionPedidos.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new DashboardViewModel
            {
                CurrentUser = currentUser,
                TotalUsers = await _unitOfWork.Users.CountAsync(),
                TotalProducts = await _unitOfWork.Products.CountAsync(),
                TotalOrders = await _unitOfWork.Orders.CountAsync(),
                PendingOrders = await _unitOfWork.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
                TotalSales = await _unitOfWork.Orders.GetTotalSalesAsync()
            };

            // Get recent orders based on user role
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Employee)
            {
                var allOrders = await _unitOfWork.Orders.GetOrdersWithItemsAsync();
                viewModel.RecentOrders = allOrders.Take(5);
            }
            else
            {
                var customerOrders = await _unitOfWork.Orders.GetByCustomerIdAsync(currentUser.Id);
                viewModel.RecentOrders = customerOrders.Take(5);
            }

            // Get low stock products (only for admin/employee)
            if (currentUser.Role == UserRole.Admin || currentUser.Role == UserRole.Employee)
            {
                var allProducts = await _unitOfWork.Products.GetAllAsync();
                viewModel.LowStockProducts = allProducts.Where(p => p.Stock <= 5);
            }

            return View(viewModel);
        }
    }
}