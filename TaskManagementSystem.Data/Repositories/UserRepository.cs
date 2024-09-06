using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;

namespace TaskManagementSystem.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetById(string id)
        {
            return _context.Users.FirstOrDefault(x => x.Id.ToString() == id);
        }

        public User GetByUsernameOrEmail(string username, string email)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username || u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
