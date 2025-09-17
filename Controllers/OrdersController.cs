using GestionPedidos.Filters;
using GestionPedidos.Models.Domain;
using GestionPedidos.Models.ViewModels;
using GestionPedidos.Repositories.Interfaces;
using GestionPedidos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionPedidos.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public OrdersController(IUnitOfWork unitOfWork, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            IEnumerable<Order> orders;

            if (currentUser.Role == UserRole.Customer)
            {
                orders = await _unitOfWork.Orders.GetByCustomerIdAsync(currentUser.Id);
            }
            else
            {
                orders = await _unitOfWork.Orders.GetOrdersWithItemsAsync();
            }

            ViewBag.CurrentUser = currentUser;
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();
            if (currentUser?.Role == UserRole.Customer && order.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            ViewBag.CurrentUser = currentUser;
            return View(order);
        }

        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateOrderViewModel
            {
                Customers = await _unitOfWork.Users.GetByRoleAsync(UserRole.Customer),
                Products = await _unitOfWork.Products.GetAllAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid && viewModel.Items.Any())
                {
                    var items = viewModel.Items.Select(i => (i.ProductId, i.Quantity)).ToList();
                    
                    if (!await _orderService.ValidateStockAvailabilityAsync(items))
                    {
                        ModelState.AddModelError("", "Stock insuficiente para uno o más productos");
                        await LoadCreateViewModelData(viewModel);
                        return View(viewModel);
                    }

                    var order = await _orderService.CreateOrderAsync(viewModel.CustomerId, items, viewModel.Notes);
                    TempData["Success"] = "Pedido creado exitosamente";
                    return RedirectToAction(nameof(Details), new { id = order.Id });
                }
                else if (!viewModel.Items.Any())
                {
                    ModelState.AddModelError("", "Debe agregar al menos un producto al pedido");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el pedido: {ex.Message}");
            }

            await LoadCreateViewModelData(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, status);
                if (success)
                {
                    TempData["Success"] = "Estado del pedido actualizado exitosamente";
                }
                else
                {
                    TempData["Error"] = "Error al actualizar el estado del pedido";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar el estado: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var order = await _orderService.GetOrderWithDetailsAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                var currentUser = GetCurrentUser();
                if (currentUser?.Role == UserRole.Customer && order.CustomerId != currentUser.Id)
                {
                    return Forbid();
                }

                if (order.Status == OrderStatus.Pending)
                {
                    var success = await _orderService.CancelOrderAsync(id);
                    if (success)
                    {
                        TempData["Success"] = "Pedido cancelado exitosamente";
                    }
                    else
                    {
                        TempData["Error"] = "Error al cancelar el pedido";
                    }
                }
                else
                {
                    TempData["Error"] = "Solo se pueden cancelar pedidos en estado Pendiente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cancelar el pedido: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductPrice(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Json(new { price = product.Price, stock = product.Stock, name = product.Name });
        }

        private async Task LoadCreateViewModelData(CreateOrderViewModel viewModel)
        {
            viewModel.Customers = await _unitOfWork.Users.GetByRoleAsync(UserRole.Customer);
            viewModel.Products = await _unitOfWork.Products.GetAllAsync();
        }
    }
}