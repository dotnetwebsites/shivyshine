using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class SuperCategory : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Category")]
        //[Required(ErrorMessage = "Please select category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Sub Category")]
        [Required(ErrorMessage = "Please select sub category")]
        public int SubCategoryId { get; set; }

        [Display(Name = "Super Category")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Super Category")]
        public string SuperCategoryName { get; set; }

        [Display(Name = "Image Url")]
        [Column(TypeName = "nvarchar(1000)")]
        public string ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Choose Image")]
        //[Required(ErrorMessage = "Please choose file")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", "jpeg", ".jpg", ".png" })]
        public IFormFile ImageFile { get; set; }

        public SubCategory SubCategory { get; set; }
        public bool IsDisplay { get; set; }
    }

    public class ExportSuperCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string SuperCategoryName { get; set; }        
    }

    public class SuperCategoryView : SuperCategory
    {
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Sub Category")]
        public string SubCategoryName { get; set; }
    }
}