using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shivyshine.Areas.Admin.Models
{
    public class VendorMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Vendor Name")]
        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Please enter vendor name")]
        public string VendorName { get; set; }

        [Display(Name = "Company Name")]
        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Please enter company name")]
        public string CompanyName { get; set; }

        [Display(Name = "Address")]
        [Column(TypeName = "nvarchar(1000)")]
        [Required(ErrorMessage = "Please enter address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string PhoneNumber { get; set; }

        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Alternate no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Alternate no must be numeric")]
        [Display(Name = "Alternate No")]
        public string AlternateNo { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }

}