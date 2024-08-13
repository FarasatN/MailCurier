using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace OtpApp
{
    public class EmailService
    {
        public async Task SendEmailAsync(string emailAddress, string password, string recipient, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailAddress, emailAddress));
            message.To.Add(new MailboxAddress(recipient, recipient));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync(emailAddress, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
