using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class ConfirmRegistrationModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int otp { get; set; }


        //public ConfirmRegistrationModel(string Email, int otp)
        //{
        //    this.Email = Email;
        //    this.otp = otp;
        //}
    }
}
