using TaskManagementSystem.Data.Models;

namespace TaskManagementSystem.Service.Interfaces
{
    public interface IAuthService
    {
        string CreateToken(User user);
    }
}