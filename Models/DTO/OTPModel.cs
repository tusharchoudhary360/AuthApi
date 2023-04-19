namespace AuthApi.Models.Domain
{
    public class OTPModel
    {
        public int otp { get; set; }
        public string emailID { get; set; }
        public string type { get; set; }
        public DateTime expiryTime { get; set; }

        public OTPModel(string emailID, string type, int otp, DateTime expiryTime)
        {
            this.emailID = emailID;
            this.type = type;
            this.otp = otp;
            this.expiryTime = expiryTime;
        }
    }
}
