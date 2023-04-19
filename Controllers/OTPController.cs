using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using AuthApi.Repositories.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OTPController : Controller
    {
        private static Dictionary<string, OTPModel> otpDictionary = new Dictionary<string, OTPModel>();

      
        private readonly IEmailService _emailService;
        private readonly DatabaseContext _context;
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService _tokenService;
        public OTPController(DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IEmailService emailService,
            IFileService fs)
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager; ///
            this._tokenService = tokenService;
            this._fileService = fs;
            this._emailService = emailService;
        }


        private int getOTP()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        private void SendEmail(OTPModel otpModel, string type)
        {
            Message message = new Message(otpModel.emailID, $"Verify OTP to Continue", $"Your OTP is {otpModel.otp} Valid Till {DateTime.Now.AddMinutes(5)}");
            _emailService.SendEmail(message);
   
            //  return Ok(new Status(200, "Email Send Success", null));
        }

        [HttpPost]
        public async Task<IActionResult> generateOtp(string emailID, string type) //type can be resetPassword or registration
        {
            if (type == "registration")
            {
                var user = await userManager.FindByEmailAsync(emailID);
                if (user != null)
                {
                    return Ok(new Status(400, "User Already Exists. Please Log in", null));
                }

                if (otpDictionary.ContainsKey(emailID)) { otpDictionary.Remove(emailID); }

                OTPModel otpModel = new OTPModel(emailID, type, getOTP(), DateTime.Now.AddMinutes(5));

                try
                {
                    SendEmail(otpModel, type); //
                }

                catch (Exception ex)
                {
                    return Ok(new Status(400, "Error Occured While Sending OTP On Your Email", null));
                }

                otpDictionary.Add(emailID, otpModel);

                return Ok(new Status(200, "OTP has been sent successfully on you email id", null));
            }

            else if (type == "resetPassword")
            {
                var user = await userManager.FindByEmailAsync(emailID);
                if (user == null)
                {
                    return Ok(new Status(400, "Invalid Email ID. User Does Not Exist", null));
                }

                //Generate OTP & Send opt to Email ID

                if (otpDictionary.ContainsKey(emailID)) { otpDictionary.Remove(emailID); } //Removing if other OTP Already Exists else will throw exception

                OTPModel otpModel = new OTPModel(emailID, type, getOTP(), DateTime.Now.AddMinutes(5));

                try
                {
                    SendEmail(otpModel, type); //
                }

                catch (Exception ex)
                {
                    return Ok(new Status(400, "Error Occured While Sending OTP On Your Email", null));
                }

                otpDictionary.Add(emailID, otpModel);

                return Ok(new Status(200, "OTP has been sent successfully on you registered email id", null));
            }

            return Ok(new Status(400, "Something Went Really Wrong!!!!!!!!!!!", null));
        }

        [HttpPost]
        public async Task<IActionResult> verifyOtp(string emailID, int otp)
        {
            if (!otpDictionary.ContainsKey(emailID))
            {
                return Ok(new Status(400, "No OTP Record Found. Please Try Again", null));
            }

            OTPModel otpModel = otpDictionary[emailID];

            if (otpDictionary[otpModel.emailID].otp == otp)//Verifying otp & Expity time
            {
                if(DateTime.Now > otpDictionary[otpModel.emailID].expiryTime)
                {
                    otpDictionary.Remove(otpModel.emailID);
                    return Ok(new Status(400, "OTP Expired", null));
                }

                otpDictionary.Remove(otpModel.emailID);
                return Ok(new Status(200, "OTP Verified", null));
            }

            return Ok(new Status(400, "Invalid OTP", null));

        }
    }
}
