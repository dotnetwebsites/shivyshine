using System.Net;
using System.Net.Mail;

namespace Shivyshine.Services
{
    public static class MailData
    {
        public static MailAddress FromAddress = new MailAddress("kpljain21@gmail.com", "Do-not-reply");
        public static string Password = "Style@1234#";
        public static string Host = "smtp.gmail.com";
        public static int Port = 587;
        public static bool EnableSsl = true;
        public static bool UseDefaultCredentials = false;
    }

}