using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.webapi.Dto
{
    public class UserForRegistrationDto
    {
        [Required(ErrorMessage="username is required")]
        public string Username { get; set; }

        [Required, StringLength(12, MinimumLength =4, ErrorMessage="password should be between 4 and 12")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastActiveDate { get; set; }

        public UserForRegistrationDto()
        {
            CreateDate = DateTime.Now;
            LastActiveDate = DateTime.Now;            
        }
    }
}