using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Data.Models;

namespace TaskManagementSystem.Data.Repositories.Contract
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByUsernameOrEmailAsync(string username, string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
