using GestionPedidos.Models.Domain;

namespace GestionPedidos.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        Task<bool> IsEmailAvailableAsync(string email);
    }
}