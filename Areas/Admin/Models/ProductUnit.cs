using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class ProductUnit : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Display(Name = " QuantityType")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter quantity type")]
        public string QuantityType { get; set; }

        [Display(Name = " Quantity")]
        [Required(ErrorMessage = "Please enter quantity")]
        public double Quantity { get; set; }

        [Display(Name = " GST IN %")]
        [Range(0, 100, ErrorMessage = "GST in percent range between 0 to 100 only.")]
        public double GSTIN { get; set; }

        [Display(Name = " MRP")]
        [Required(ErrorMessage = "Please enter MRP")]
        public double MRP { get; set; }

        [Display(Name = " Discount in Rupees")]
        public double? DiscountInRs { get; set; }

        [Display(Name = " Discount in %")]
        [Range(0, 100, ErrorMessage = "Discount percent range between 0 to 100 only.")]
        public double? DiscountInPer { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public string ProductName { get; set; }

        [NotMapped]
        public IFormFile ProductUnitImages { get; set; }

        [NotMapped]
        [Display(Name = "Product Image")]
        public string ProductUnitImageUrl { get; set; }

        [NotMapped]
        public double Price
        {
            get
            {
                if (DiscountInPer != null)
                {
                    return MRP - (MRP * (double)DiscountInPer / 100);
                }
                else if (DiscountInRs != null)
                {
                    return MRP - (double)DiscountInRs;
                }
                else
                    return MRP;
            }
        }

        [NotMapped]
        public double GSTCal
        {
            get
            {
                return (Price * Quantity) - ((Price * Quantity) * (100 / (100 + GSTIN)));
            }
        }

        [NotMapped]
        public double PriceDel
        {
            get
            {
                if (DiscountInPer != null || DiscountInRs != null)
                {
                    return MRP;
                }
                else
                    return 0;
            }
        }

        [NotMapped]
        public string GetPacking
        {
            get
            {
                return Quantity + " " + QuantityType;
            }
        }

        public double AmountCalc(double price, double quantity)
        {
            return price * quantity;
        }

        public double GSTCalc(double amount, double gstpercent)
        {
            return amount - (amount * (100 / (100 + GSTIN)));
        }
    }

    public class ExportProductUnit
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double MRP { get; set; }

        [Display(Name = "Discount in Rs")]
        public double? DiscountInRs { get; set; }

        [Display(Name = "Discount in %")]
        public double? DiscountInPer { get; set; }


        public double GSTIN { get; set; }
        public double Price { get; set; }
        //public double GSTCal { get; set; }
        public string GetPacking { get; set; }
        public bool IsActive { get; set; }

    }
}