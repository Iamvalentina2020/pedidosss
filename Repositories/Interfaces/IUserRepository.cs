using GestionPedidos.Models.Domain;

namespace GestionPedidos.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    }
}