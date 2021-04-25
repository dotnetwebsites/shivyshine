using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Areas.Admin.Models
{
    public class Brand : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Brand Name")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter brand name")]
        public string BrandName { get; set; }
    }

    public class ExportBrand
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
    }
}