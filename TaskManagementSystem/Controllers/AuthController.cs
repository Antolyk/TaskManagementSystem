using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Service.Interfaces;
using TaskManagementSystem.Service.Models;

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

        [HttpPost("users/register")]
        public async Task<ActionResult<UserServiceModel>> Register(UserDto request)
        {
            UserServiceModel existingUser = _userService.GetByUsernameOrEmail(request);
            if (existingUser != null)
                return BadRequest("User with the same username or email already exists.");
            
            UserServiceModel user = await _userService.RegisterUserAsync(request);

            return Ok(user);
        }

        [HttpPost("users/login")]
        public ActionResult<string> Login(UserDto request)
        {
            UserServiceModel existingUser = _userService.GetByUsernameOrEmail(request);
            if (existingUser == null)
                return BadRequest("User with that username/email not found.");
            
            if (!_userService.CheckUserPassword(request))
                return BadRequest("Wrong password!");
            
            // JWT token for user
            var token = _authService.CreateToken(existingUser);

            return Ok(token);
        }
    }
}
