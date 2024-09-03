using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
