using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories;

namespace TaskManagementSystem.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        #region AddAsync
        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            // Act
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Assert
            var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal("testuser", retrievedUser.Username);
        }

        [Fact]
        public async Task AddAsync_ShouldFailToAddUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            // Act
            await _userRepository.AddAsync(user);
            // Intentionally skip SaveChangesAsync to simulate failure

            // Assert
            var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.Null(retrievedUser); // This should fail because SaveChangesAsync was not called
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Act
            var retrievedUser = _userRepository.GetById(user.Id.ToString());

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
            Assert.Equal("testuser", retrievedUser.Username);
        }

        [Fact]
        public async Task GetById_ShouldFailToReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Act
            var retrievedUser = _userRepository.GetById(Guid.NewGuid().ToString()); // Use a different ID

            // Assert
            Assert.Null(retrievedUser); // This should pass because ID does not exist
        }
        #endregion

        #region GetByUsernameOrEmail
        [Fact]
        public async Task GetByUsernameOrEmail_ShouldReturnUserByUsername()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Act
            var retrievedUser = _userRepository.GetByUsernameOrEmail("testuser", null);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Username, retrievedUser.Username);
        }

        [Fact]
        public async Task GetByUsernameOrEmail_ShouldReturnUserByEmail()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@example.com",
                PasswordHash = "hashedpassword"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Act
            var retrievedUser = _userRepository.GetByUsernameOrEmail(null, "testuser@example.com");

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Email, retrievedUser.Email);
        }

        [Fact]
        public async Task GetByUsernameOrEmail_ShouldReturnNullWhenUserNotFound()
        {
            // Act
            var retrievedUser = _userRepository.GetByUsernameOrEmail("nonexistentuser", "nonexistent@example.com");

            // Assert
            Assert.Null(retrievedUser);
        }
        #endregion
    }
}
