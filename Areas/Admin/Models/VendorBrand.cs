using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shivyshine.Areas.Admin.Models
{
    public class VendorBrand : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Vendor")]
        [Required(ErrorMessage = "Please select vendor")]
        public int VendorId { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Please select brand")]
        public int BrandId { get; set; }

        public double Margin { get; set; }
    }

    public class VendorBrandView : VendorBrand
    {
        public string VendorName { get; set; }

        public string BrandName { get; set; }
    }

}