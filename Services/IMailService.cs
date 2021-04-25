using System.Threading.Tasks;

namespace Shivyshine.Services
{
    public interface IMailService
    {
        void SendEmail(string toEmail, string subject, string content);
        void SendEmail(string toEmail, string name, string subject, string content);
        void SendEmail(Mail mailUserId, string toEmail, string name, string subject, string content);
    }
}