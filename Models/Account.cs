using System;
using System.ComponentModel.DataAnnotations;

namespace PenjualanOnline.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string Role { get; set; } = "user";
    }
}
