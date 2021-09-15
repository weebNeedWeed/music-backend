using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace musicbackend.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Username length must be less than 50 and greater than 8")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password length must be less than 50 and greater than 8")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage ="Confirm password must match password")]
        public string ConfirmPassword { get; set; }
    }
}
