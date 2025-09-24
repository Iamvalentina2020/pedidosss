using GestionPedidos.Filters;
using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionPedidos.Controllers
{
    [Authorize(UserRole.Admin)]
    public class UsersController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if email already exists
                    var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Este email ya está registrado");
                        return View(user);
                    }

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Usuario creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al crear el usuario: {ex.Message}");
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Check if email already exists for another user
                    var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        ModelState.AddModelError("Email", "Este email ya está registrado por otro usuario");
                        return View(user);
                    }

                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Usuario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el usuario: {ex.Message}");
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user != null)
                {
                    user.IsActive = false; // Soft delete
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                    TempData["Success"] = "Usuario eliminado exitosamente";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}