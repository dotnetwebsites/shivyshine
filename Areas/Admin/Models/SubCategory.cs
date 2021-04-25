using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class SubCategory : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }

        [Display(Name = "Sub Category")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Sub Category")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Image Url")]
        [Column(TypeName = "nvarchar(1000)")]
        public string ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Choose Image")]
        //[Required(ErrorMessage = "Please choose file")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".pdf", "jpeg", ".jpg", ".png" })]
        public IFormFile ImageFile { get; set; }

        public Category Category { get; set; }
        public ICollection<SuperCategory> SuperCategories { get; set; }
    }

    public class ExportSubCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
    }


    public class SubCategoryView : SubCategory
    {
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}