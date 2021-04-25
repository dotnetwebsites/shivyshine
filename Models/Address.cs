
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class Address : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }

        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter pincode")]
        [Range(100000, 999999, ErrorMessage = "Pincode is not valid, please enter six digit pincode.")]
        public int Pincode { get; set; }

        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter full address")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Full Address")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Please enter landmark")]
        [DataType(DataType.Text)]
        [Display(Name = "Landmark")]
        public string Landmark { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + Lastname;
            }
        }

    }
}