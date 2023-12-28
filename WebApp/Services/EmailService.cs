using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebApp.Settings;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SmtpSetting> smtpSetting;

        public EmailService(IOptions<SmtpSetting> _smtpSetting) //injecting smtp setting with asp.net core options 
        {
            smtpSetting = _smtpSetting;
        }


        //mail from,to,subject and message

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from,
                   to,
                   subject,
                   body);

            // smtp client to send mail msg out
            using (var emailclient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailclient.Credentials = new NetworkCredential(
                    smtpSetting.Value.User,
                    smtpSetting.Value.Password);
                await emailclient.SendMailAsync(message);

            }
        }
    }
}
