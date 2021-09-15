using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace musicbackend.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }
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
        public ICollection<Music> Musics { get; set; }
    }
}
