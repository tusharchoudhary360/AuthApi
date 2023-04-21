using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
