using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class OrderDelivery : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public int CustId { get; set; }
        public string Username { get; set; }
        public string DeliveryStatus { get; set; }
        public string Remark { get; set; }
    }
}