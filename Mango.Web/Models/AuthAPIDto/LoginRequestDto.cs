using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.AuthAPIDto
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
