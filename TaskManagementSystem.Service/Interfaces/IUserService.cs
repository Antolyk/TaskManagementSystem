using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Data.Models;


namespace TaskManagementSystem.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(UserDto request);
        Task<User> GetByUsernameOrEmailAsync(UserDto request);
        Task<bool> CheckUserPassword(UserDto request);
        Task<User> AuthenticateUser(string usernameOrEmail, string password);
        Task<User> GetUserById(Guid userId);
    }
}
