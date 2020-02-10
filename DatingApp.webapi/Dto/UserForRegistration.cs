using System.ComponentModel.DataAnnotations;

namespace DatingApp.webapi.Dto
{
    public class UserForRegistrationDto
    {
        [Required(ErrorMessage="username is required")]
        public string Username { get; set; }

        [Required, StringLength(8, MinimumLength =4, ErrorMessage="password should be between 4 and 8")]
        public string Password { get; set; }
    }
}