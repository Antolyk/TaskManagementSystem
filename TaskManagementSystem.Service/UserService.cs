using Azure.Core;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;
using TaskManagementSystem.Service.Interfaces;
using TaskManagementSystem.Service.Models;

namespace TaskManagementSystem.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        public async Task<UserServiceModel> RegisterUserAsync(UserDto request)
        {
            _logger.LogInformation("Registering user with username: {Username}", request.Username);

            try
            {
                UserServiceModel existingUser = GetByUsernameOrEmail(request);
                if (existingUser != null)
                {
                    _logger.LogError("User {Username} is existed already.", request.Username);
                    return null;
                }

                // Hash user's password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create an entity
                User user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully.", request.Username);

                return new UserServiceModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {Username}.", request.Username);
                throw;
            }
        }

        public UserServiceModel GetByUsernameOrEmail(UserDto request)
        {
            _logger.LogInformation("Returning user model with username: {Username} and Email: {Email}", request.Username, request.Email);

            try
            {
                User user = _userRepository.GetByUsernameOrEmail(request.Username, request.Email);
                if (user == null)
                {
                    _logger.LogError("User {Username} is null.", request.Username);
                    return null;
                }

                // Create a response model
                UserServiceModel userModel = new UserServiceModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                _logger.LogInformation("Returning user model with username: {Username} and Email: {Email} was succesfull", request.Username, request.Email);
                return userModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding and returning user {Username}.", request.Username);
                throw;
            }
        }

        public bool CheckUserPassword(UserDto request)
        {
            _logger.LogInformation("Checking and verifying password {Password} with username: {Username}", request.Password, request.Username);

            try
            {
                User user = _userRepository.GetByUsernameOrEmail(request.Username, request.Email);

                if (user == null)
                {
                    _logger.LogError("User {Username} is null.", request.Username);
                    return false;
                }

                // Verify password
                bool checkResult = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                _logger.LogInformation("User {Username} was succesfully checked. The result is {checkResult}", request.Username, checkResult);
                return checkResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while verifying user {Username}.", request.Username);
                throw;
            }
        }

        public bool CheckUserById(string userId)
        {
            _logger.LogInformation("Checking if user with Id: {userId} exist", userId);
            try
            {
                var user = _userRepository.GetById(userId);

                if (user == null)
                {
                    _logger.LogError("User {userId} is null.", userId);
                    return false;
                }

                _logger.LogInformation("User {userId} was succesfully checked", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking user {userId}.", userId);
                throw;
            }
        }
    }
}
