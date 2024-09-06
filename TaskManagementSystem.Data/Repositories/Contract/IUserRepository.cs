using TaskManagementSystem.Data.Models;

namespace TaskManagementSystem.Data.Repositories.Contract
{
    public interface IUserRepository
    {
        User GetById(string id);
        User GetByUsernameOrEmail(string username, string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
