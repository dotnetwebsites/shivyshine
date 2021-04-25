using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shivyshine.Areas.Admin.Models
{
    public class Pincode : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Please choose City")]
        public int CityId { get; set; }

        [Display(Name = "Pincode")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Pincode")]
        public string Pincodes { get; set; }
    }

    public class ExportPincode
    {
        public int Id { get; set; }
        public string CityName { get; set; }
        public string Pincodes { get; set; }
    }

    public class PincodeView : Pincode
    {
        [Display(Name = "City")]
        public string CityName { get; set; }
    }
}