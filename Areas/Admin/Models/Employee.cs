using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Models
{
    public class Employee : SchemaLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Employee Code")]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter employee code")]
        public string EmpCode { get; set; }


        [Display(Name = "First Name")]
        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Column(TypeName = "nvarchar(200)")]
        public string LastName { get; set; }

        [Display(Name = "Father Name")]
        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Please enter father name")]
        public string FatherName { get; set; }

        [Display(Name = "Gender")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please enter gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Required Date of Birth")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date, ErrorMessage = "Required Date of Birth")]
        [DateOfBirth(MinAge = 18, MaxAge = 150, ErrorMessage = "Eligibility above 18 years only.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Required(ErrorMessage = "Please enter username")]
        [Display(Name = "Username")]
        [MinLength(5, ErrorMessage = "Minimum 5 characters required in username")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Space not allowed, please enter valid username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [Column(TypeName = "nvarchar(1000)")]
        public string Address { get; set; }

        [Display(Name = "Working Location")]
        [Column(TypeName = "nvarchar(500)")]
        [Required(ErrorMessage = "Please enter working location")]
        public string WorkLocation { get; set; }

        [Display(Name = "Marital Status")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Please enter marital status")]
        public string MaritialStatus { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Anniversary { get; set; }
        public bool IsActive { get; set; }

        public string Password { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class ERole : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class EmployeeRole : SchemaLog
    {
        public int RoleId { get; set; }
        public int EmpId { get; set; }
    }

}