using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class MainBanner : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter banner title")]
        [Display(Name = "Banner Title")]
        public string BannerTitle { get; set; }

        public string BannerUrl { get; set; }

        [NotMapped]
        [Display(Name = "Banner Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile BannerImage { get; set; }

        [Display(Name = "URL Location")]
        public string RedirectedUrl { get; set; }

    }

}