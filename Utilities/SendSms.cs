using System;

namespace Shivyshine.Utilities
{
    public class SendSms
    {
        public string Message { get; set; }

        public SendSms()
        {
            Message = "default sms";
        }

        public SendSms(string orderNo, DateTime orderDate)
        {
            Message = "Dear Customer,\n" +
            "Thank you for shopping with Shivyshine,\n" +
            "Your order has been placed with Order No : " + orderNo +
            " on " + orderDate.ToString("dd-MMM-yyyy");
        }

    }
}