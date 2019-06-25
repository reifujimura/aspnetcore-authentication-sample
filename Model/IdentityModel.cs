using System.ComponentModel.DataAnnotations;

namespace AuthenticationSample.Model
{
    public class IdentityModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}