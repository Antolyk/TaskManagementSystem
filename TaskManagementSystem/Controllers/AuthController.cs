using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Service.Interfaces;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            // Перевірка чи користувач з таким ім'ям або емейлом вже існує
           var existingUser = await _userService.GetByUsernameOrEmailAsync(request);
           if (existingUser != null)
           {
               return BadRequest("User with the same username or email already exists.");
           }

            // Реєстрація нового користувача
            var user = await _userService.RegisterUserAsync(request);

            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            // Перевірка чи користувач з таким ім'ям або емейлом вже існує
            var existingUser = await _userService.GetByUsernameOrEmailAsync(request);
            if (existingUser == null)
            {
                return BadRequest("User with that username/email not found.");
            }

            //
            if (!_userService.CheckUserPassword(request).Result)
            {
                return BadRequest("Wrong password!");
            }

            // Створення JWT токену
            var token = _authService.CreateToken(existingUser);

            return Ok(token);
        }
    }
}
