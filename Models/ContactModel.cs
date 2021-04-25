using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class ContactModel : SchemaLog
    {

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Enter Your Email")]
        [Required(ErrorMessage = "Please enter email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter subject")]
        public string Subject { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Message { get; set; }

        public int? Status { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Remark { get; set; }

    }

    public class ContactUSView : ContactModel
    {

        [DataType(DataType.Text)]
        public string Address
        {
            get
            {
                return "45, Dwarka Nagar, Coach Factory Road, Bhopal, India";
            }
        }

        [DataType(DataType.Text)]
        public string ContactPerson
        {
            get
            {
                return "Mobile: (+91) 888 999 1664 - Vikas Souraiya";
            }
        }

        //Get email by semicolon separated
        public string Emails
        {
            get
            {
                return "info@shivyshine.com;";
            }
        }

    }


}