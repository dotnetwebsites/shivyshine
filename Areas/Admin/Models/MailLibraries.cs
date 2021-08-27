using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Models
{
    public class MailLibrary
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "ID")]
        [Required(ErrorMessage = "Please enter ID")]
        public string MailUserId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter Email ID")]
        [Display(Name = "From Email ID")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Display(Name = "Person Name")]
        [Required(ErrorMessage = "Please person name")]
        public string EmailName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Server/Host")]
        [Required(ErrorMessage = "Please enter server/host")]
        public string Host { get; set; }

        [Required(ErrorMessage = "Please enter port no")]
        public int Port { get; set; }

        [Display(Name = "Enable SSL")]
        public bool EnableSsl { get; set; }

        [Display(Name = "Use default credentials")]
        public bool UseDefaultCredentials { get; set; }
        public bool System { get; set; }
        // [NotMapped]
        // public MailAddress FromAddress = new MailAddress(EmailAddress, EmailName);
    }

    public class MailLibraryView
    {
        [Display(Name = "Mail Type")]
        [Required(ErrorMessage = "Please enter Mail Type")]
        public Mail MailType { get; set; }

        [Display(Name = "To Email Address")]
        [Required(ErrorMessage = "Please enter From email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Person Name")]
        [Required(ErrorMessage = "Please enter person name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Please enter message")]
        public string Content { get; set; }
    }

}