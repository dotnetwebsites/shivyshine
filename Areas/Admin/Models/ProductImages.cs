using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class ProductImage : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select product")]
        public int ProductId { get; set; }

        public int? ShadeId { get; set; }
        public int? ProductUnitId { get; set; }

        [Display(Name = "Main Image")]
        public bool IsMainPic { get; set; }
        public string ProductImageUrl { get; set; }
    }

    public class ProductImageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select product first")]
        [RequiredNotNull(ErrorMessage = "Please select")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please choose product image")]
        [Display(Name = "Product Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".zip" })]
        public IFormFile ProductImage { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Description")]
        public string ProdDesc { get; set; }

        [Display(Name = "Product Image URL")]
        public string ProductImageUrl { get; set; }

        [Display(Name = "HSN Code")]
        public string HSNCode { get; set; }
        public string Brand { get; set; }
        public float MRP { get; set; }
        public float Rate { get; set; }
        public float Qty { get; set; }

        public bool IsActive { get; set; }
        public virtual List<ProductImage> ProductImages { get; set; }
    }


}