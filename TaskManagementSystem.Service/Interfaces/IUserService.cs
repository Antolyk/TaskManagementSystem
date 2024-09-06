using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Service.Models;


namespace TaskManagementSystem.Service.Interfaces
{
    public interface IUserService
    {
        Task<UserServiceModel> RegisterUserAsync(UserDto request);
        UserServiceModel GetByUsernameOrEmail(UserDto request);
        bool CheckUserPassword(UserDto request);
    }
}
