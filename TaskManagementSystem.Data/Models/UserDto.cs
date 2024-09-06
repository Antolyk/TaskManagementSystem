namespace TaskManagementSystem.Data.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        [PasswordComplexity]
        public required string Password { get; set; }
    }
}
