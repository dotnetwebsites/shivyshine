using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        //[Required(ErrorMessage = "Please enter date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [PersonalData]
        //[Required(ErrorMessage = "Select Gender")]
        public bool Gender { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(500)")]
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [NotMapped]
        [Display(Name = "Profile Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        //[Required(ErrorMessage = "Please choose profile image")]
        public IFormFile Avatar { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}