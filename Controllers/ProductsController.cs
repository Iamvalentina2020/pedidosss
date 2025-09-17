using GestionPedidos.Filters;
using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionPedidos.Controllers
{
    [Authorize]
    public class ProductsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? search, string? category, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<Product> products = await _unitOfWork.Products.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                products = await _unitOfWork.Products.SearchByNameAsync(search);
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            if (minPrice.HasValue || maxPrice.HasValue)
            {
                var min = minPrice ?? 0;
                var max = maxPrice ?? decimal.MaxValue;
                products = products.Where(p => p.Price >= min && p.Price <= max);
            }

            ViewBag.Categories = await _unitOfWork.Products.GetCategoriesAsync();
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentUserRole = GetCurrentUser()?.Role.ToString();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(UserRole.Admin, UserRole.Employee)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _unitOfWork.Products.AddAsync(product);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Producto creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el producto: {ex.Message}");
            }
            return View(product);
        }

        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [Authorize(UserRole.Admin, UserRole.Employee)]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el producto: {ex.Message}");
            }
            return View(product);
        }

        [Authorize(UserRole.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(UserRole.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product != null)
                {
                    product.IsActive = false; 
                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Producto eliminado exitosamente";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}