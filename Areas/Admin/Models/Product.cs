using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class Product : SchemaLog
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "Brand Name")]
        [Required(ErrorMessage = "Please select brand")]
        public int BrandId { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }

        [Display(Name = " Sub Category")]
        [Required(ErrorMessage = "Please select sub category")]
        public int SubCategoryId { get; set; }

        [Display(Name = " Super Category")]
        [Required(ErrorMessage = "Please select super category")]
        public int SuperCategoryId { get; set; }

        [Display(Name = " Product Name")]
        [Required(ErrorMessage = "Please enter product name")]
        [Column(TypeName = "nvarchar(500)")]
        public string ProductName { get; set; }

        [Display(Name = " Specification")]
        [Column(TypeName = "nvarchar(2000)")]
        public string Specification { get; set; }

        [Display(Name = " Discription")]
        [Column(TypeName = "nvarchar(2000)")]
        public string Discription { get; set; }

        [Display(Name = " SpecialNotes")]
        [Column(TypeName = "nvarchar(2000)")]
        public string SpecialNotes { get; set; }

        [Display(Name = " How to use")]
        [Column(TypeName = "nvarchar(500)")]
        public string HowToUse { get; set; }

        [Display(Name = " Shades/Color")]
        [Column(TypeName = "nvarchar(500)")]
        public string Shades { get; set; }

        // [Display(Name = " QuantityType")]
        // [Column(TypeName = "nvarchar(100)")]
        // [Required(ErrorMessage = "Please enter quantity type")]
        // public string QuantityType { get; set; }

        // [Display(Name = " Quantity")]
        // [Required(ErrorMessage = "Please enter quantity")]
        // public int Quantity { get; set; }

        [Display(Name = " HSN/SAC Code")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter HSN/SAC Code")]
        public string HsnCode { get; set; }

        // [Display(Name = " GST IN %")]
        // [Range(0, 100, ErrorMessage = "GST in percent range between 0 to 100 only.")]
        // public double GSTIN { get; set; }

        // [Display(Name = " MRP")]
        // [Required(ErrorMessage = "Please enter MRP")]
        // public double MRP { get; set; }

        // [Display(Name = " Discount in Rupees")]
        // public double? DiscountInRs { get; set; }

        // [Display(Name = " Discount in %")]
        // [Range(0, 100, ErrorMessage = "Discount percent range between 0 to 100 only.")]
        // public double? DiscountInPer { get; set; }

        public bool IsActive { get; set; }

        // [NotMapped]
        // //[Required(ErrorMessage = "Please choose product image")]
        // [Display(Name = "Product Image")]
        // [DataType(DataType.Upload)]
        // [AllowedExtensions(new string[] { ".zip" })]
        // public IFormFile ProductImage { get; set; }

        [NotMapped]
        public bool IsPreviewSize { get; set; }

        [NotMapped]
        public bool IsPreviewShade { get; set; }

        public bool IsDisplay { get; set; }

    }

    public class ExportProduct
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string SuperCategory { get; set; }

        public string ProductName { get; set; }

        public string HsnCode { get; set; }

        public string Specification { get; set; }
        public string HowToUse { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisplay { get; set; }

    }

    public class ProductView : Product
    {
        [Display(Name = "Brand")]
        public string BrandName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Sub Category")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Super Category")]
        public string SuperCategoryName { get; set; }

        [Display(Name = "Product Image")]
        public string ProductImageUrl { get; set; }
    }
}