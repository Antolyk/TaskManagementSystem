﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Data.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual IEnumerable<TaskItem> Tasks { get; set; }
    }

}
