using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using System.Net;
using System.Net.Mail;

namespace AuthApi.Repositories.Domain
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig) => _emailConfig = emailConfig;
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MailMessage CreateEmailMessage(Message message)
        {
            var EmailMessage = new MailMessage();
            EmailMessage.From = new MailAddress(_emailConfig.From);
            EmailMessage.To.Add(message.To);
            EmailMessage.Subject = message.Subject;
            EmailMessage.Body = message.Content;

            //EmailMessage.Attachments.Add(new Attachment(model.FileAtt.OpenReadStream(), model.FileAtt.FileName));
            EmailMessage.IsBodyHtml = false;
            return EmailMessage;
        }

        private void Send(MailMessage mailMessage)
        {
            using var client = new SmtpClient(_emailConfig.SmtpServer);
            try
            {
                client.Port = _emailConfig.Port;
                client.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
                client.EnableSsl = true;
                client.Send(mailMessage);
            }
            catch
            {
                throw;
            }
            finally
            {
                client.Dispose();

            }
        }
    }
}
