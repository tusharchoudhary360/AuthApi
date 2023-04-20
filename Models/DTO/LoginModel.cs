using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class LoginModel
    {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            public string? Password { get; set; }
    }
}
