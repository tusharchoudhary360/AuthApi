using AuthApi.Models.Domain;

namespace AuthApi.Repositories.Abstract
{
    public interface IOtpService
    {
        Task<bool> generateOtp(string emailID, string type);
        Task<bool> verifyOtp(OTPModel otpModel);
    }
}
