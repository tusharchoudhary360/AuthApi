using AuthApi.Models.DTO;

namespace AuthApi.Repositories.Abstract
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
