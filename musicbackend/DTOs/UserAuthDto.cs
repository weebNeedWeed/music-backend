﻿using System.ComponentModel.DataAnnotations;

namespace musicbackend.DTOs
{
    public class UserAuthDto
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
