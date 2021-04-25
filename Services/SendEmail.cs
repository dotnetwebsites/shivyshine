using System.Linq;
using System.Net;
using System.Net.Mail;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Microsoft.AspNetCore.Identity;

namespace Shivyshine.Services
{
    public class SendMail : IMailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _repository;
        public SendMail(UserManager<ApplicationUser> userManager, ApplicationDbContext repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        public void SendEmail(string toEmail, string name, string subject, string content)
        {
            var toAddress = new MailAddress(toEmail, name);

            var smtp = new SmtpClient
            {
                Host = MailData.Host,
                Port = MailData.Port,
                EnableSsl = MailData.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = MailData.UseDefaultCredentials,
                Credentials = new NetworkCredential(MailData.FromAddress.Address, MailData.Password)
            };

            using (var message = new MailMessage(MailData.FromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendEmail(Mail mailUserId, string toEmail, string name, string subject, string content)
        {
            var maildata = _repository.MailLibraries.Where(p => p.MailUserId == mailUserId.ToString()).FirstOrDefault();
            MailAddress fromAddress = new MailAddress(maildata.EmailAddress, maildata.EmailName);

            if (maildata != null)
            {
                var toAddress = new MailAddress(toEmail, name);

                var smtp = new SmtpClient
                {
                    Host = maildata.Host,
                    Port = maildata.Port,
                    EnableSsl = maildata.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = maildata.UseDefaultCredentials,
                    Credentials = new NetworkCredential(fromAddress.Address, maildata.Password)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
        }

        public void SendEmail(string toEmail, string subject, string content)
        {
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = MailData.Host,
                Port = MailData.Port,
                EnableSsl = MailData.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = MailData.UseDefaultCredentials,
                Credentials = new NetworkCredential(MailData.FromAddress.Address, MailData.Password)
            };

            using (var message = new MailMessage(MailData.FromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

    }

}
