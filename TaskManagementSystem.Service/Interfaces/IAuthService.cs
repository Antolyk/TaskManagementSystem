using TaskManagementSystem.Service.Models;

namespace TaskManagementSystem.Service.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(UserServiceModel user);
    }
}