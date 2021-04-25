using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class Shade : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Product")]
        [Required(ErrorMessage = "Select product")]
        public int ProductId { get; set; }
        
        [Display(Name = "Product Unit")]
        [Required(ErrorMessage = "Select Product Unit")]
        public int ProductUnitId { get; set; }

        [Display(Name = "Shade name")]
        [Required(ErrorMessage = "Please enter shade name")]
        public string ShadeName { get; set; }

        [Display(Name = "Shade color")]
        [Required(ErrorMessage = "Please enter shade color")]
        public string ShadeColor { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public IFormFile ShadesImages { get; set; }

        [NotMapped]
        public string ProductName { get; set; }

        [NotMapped]
        [Display(Name = "Product Image")]
        public string ProductShadeImageUrl { get; set; }

        //newly added
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

        public bool IsVisiblePrice { get; set; }

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

        public double AmountCalc(double price, double quantity)
        {
            return price * quantity;
        }

        public double GSTCalc(double amount, double gstpercent)
        {
            return amount - (amount * (100 / (100 + GSTIN)));
        }
    }

    public class ExportShade
    {
        public int Id { get; set; }

        public string ProductName { get; set; }
        
        public string ProductUnit { get; set; }

        public string ShadeName { get; set; }

        public string ShadeColor { get; set; }
        public bool IsActive { get; set; }        
    }

}