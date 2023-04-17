using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class RefreshTokenRequest
    {
        [Required]
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}
