using System.ComponentModel.DataAnnotations;

namespace DatingApp.webapi.Dto
{
    public class UserForLoginDto
    {
        [Required]
        [StringLength(20, MinimumLength=4)]
        public string Username { get; set; }
        [Required]
        [StringLength(20, MinimumLength=4, ErrorMessage ="password should be atleast 4 characaters long.")]
        public string Password { get; set; }
    }
}