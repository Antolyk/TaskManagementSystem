using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories;
using TaskManagementSystem.Data.Repositories.Contract;
using TaskManagementSystem.Service.Interfaces;

namespace TaskManagementSystem.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUserAsync(UserDto request)
        {
            string passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordhash,
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetByUsernameOrEmailAsync(UserDto request)
        {
            return await _userRepository.GetByUsernameOrEmailAsync(request.Username, request.Email);
        }

        public async Task<bool> CheckUserPassword(UserDto request)
        {
            var user = await GetByUsernameOrEmailAsync(request);
            return BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        }

        public Task<User> AuthenticateUser(string usernameOrEmail, string password)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}
