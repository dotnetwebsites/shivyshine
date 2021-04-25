using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class ShippingMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select pincode")]
        [Display(Name = "Pincode")]
        public int Pincode { get; set; }

        [Required(ErrorMessage = "Please enter minimum cut off amount")]
        [Display(Name = "Minimum Amount")]
        public double MinAmount { get; set; }

        [Required(ErrorMessage = "Please enter shipping charges")]
        [Display(Name = "Shipping Charge")]
        public double ShippingCharge { get; set; }

        [Required(ErrorMessage = "Please select date")]
        [Display(Name = "Date")]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Delivered By")]
        public string DeliveredBy { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}