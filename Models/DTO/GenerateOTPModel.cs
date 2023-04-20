using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class GenerateOTPModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? type { get; set; }


        public GenerateOTPModel(string Email, string type)
        {
            this.Email = Email;
            this.type = type;
        }

    }
}
