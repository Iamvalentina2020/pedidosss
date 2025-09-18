using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using GestionPedidos.Services.Interfaces;

namespace GestionPedidos.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            return await _unitOfWork.Users.ValidateUserAsync(email, password);
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                if (await _unitOfWork.Users.GetByEmailAsync(user.Email) != null)
                    return false;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(email);
            return existingUser == null;
        }
    }
}