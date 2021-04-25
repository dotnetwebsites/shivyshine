using System.ComponentModel.DataAnnotations;

namespace Shivyshine.Models
{
    public class Account
    {

    }

    public class SendEmailConfirmLinkViewModel
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "Space not allowed, please enter valid username or email")]
        [Required(ErrorMessage = "Please enter username or email address.")]
        [Display(Name = "Email Address or Username")]
        public string Value { get; set; }

        public bool IsSuccess { get; set; }
    }
}