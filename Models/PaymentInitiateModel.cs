
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class PaymentInitiateModel : SchemaLog
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }

        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public double ConfirmAmount { get; set; }
        public string Currency { get; set; }
        
    }
}