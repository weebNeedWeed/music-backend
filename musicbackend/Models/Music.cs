using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace musicbackend.Models
{
    public class Music
    {
        [Key]
        public int MusicId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string YoutubeUrl { get; set; }
        [Required]
        public string UploaderName { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
