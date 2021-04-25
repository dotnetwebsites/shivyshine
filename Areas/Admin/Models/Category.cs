using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class Category : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Category")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Image Url")]
        [Column(TypeName = "nvarchar(1000)")]
        public string ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Choose Image")]
        //[Required(ErrorMessage = "Please choose file")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", "jpeg", ".jpg", ".png" })]
        public IFormFile ImageFile { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
    }

    public class ExportCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }

}