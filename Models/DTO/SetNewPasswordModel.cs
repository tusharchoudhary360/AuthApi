﻿using System.ComponentModel.DataAnnotations;

namespace PhotosWebApp.Models.Users
{
    public class SetNewPasswordModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public int Otp { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string NewPassword { get; set; }
    }
}
