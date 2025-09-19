using GestionPedidos.Data;
using GestionPedidos.Models.Domain;
using GestionPedidos.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPedidos.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbSet.Where(u => u.Role == role && u.IsActive).ToListAsync();
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.Where(u => u.IsActive).ToListAsync();
        }
    }
}