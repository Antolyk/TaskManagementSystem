using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;
using TaskManagementSystem.Service;

namespace TaskManagementSystem.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
        }

        #region RegisterUserAsync
        [Fact]
        public async Task RegisterUserAsync_ShouldReturnUserModel_WhenCorrectUserDto()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldFail_WhenUserAlreadyExists()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "Test@1234"
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new User { Username = "existinguser" });

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetByUsernameOrEmail
        [Fact]
        public void GetByUsernameOrEmail_ShouldReturnUserModel_WhenUserExists()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "Existing-1"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user);

            // Act
            var result = _userService.GetByUsernameOrEmail(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PasswordHash, result.PasswordHash);
            _userRepositoryMock.Verify(x => x.GetByUsernameOrEmail(userDto.Username, userDto.Email), Times.Once);
        }

        [Fact]
        public void GetByUsernameOrEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "nonexistentuser",
                Email = "nonexistent@example.com",
                Password = "Nonexisting-1"
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((User)null);

            // Act
            var result = _userService.GetByUsernameOrEmail(userDto);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(x => x.GetByUsernameOrEmail(userDto.Username, userDto.Email), Times.Once);
        }
        #endregion

        #region CheckUserPassword
        [Fact]
        public void CheckUserPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user);

            // Act
            var result = _userService.CheckUserPassword(userDto);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(x => x.GetByUsernameOrEmail(userDto.Username, userDto.Email), Times.Once);
        }

        [Fact]
        public void CheckUserPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(user);

            // Act
            var result = _userService.CheckUserPassword(userDto);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(x => x.GetByUsernameOrEmail(userDto.Username, userDto.Email), Times.Once);
        }

        [Fact]
        public void CheckUserPassword_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "nonexistentuser",
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((User)null);

            // Act
            var result = _userService.CheckUserPassword(userDto);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(x => x.GetByUsernameOrEmail(userDto.Username, userDto.Email), Times.Once);
        }
        #endregion
    }
}
