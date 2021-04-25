using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Models;

namespace Shivyshine.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmPhoneNumberChangeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogToken _repository;
        private readonly ILogger<ConfirmPhoneNumberChangeModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "One time password")]
            [MaxLength(6)]
            [MinLength(6, ErrorMessage = "Invalid OTP")]
            [Required(ErrorMessage = "OTP not entered")]
            public string OTP { get; set; }
        }

        public ConfirmPhoneNumberChangeModel(UserManager<ApplicationUser> userManager,
                                            SignInManager<ApplicationUser> signInManager,
                                            ILogToken repository,
                                            ILogger<ConfirmPhoneNumberChangeModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repository = repository;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string phoneNumber)
        {
            if (userId == null || phoneNumber == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (await _userManager.IsPhoneNumberConfirmedAsync(user))
            {
                return RedirectToAction("UserProfile", "Account");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (user != null)
            {
                string code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Input.OTP));

                var token = await _repository.GetLogTokenAsync(user.Id, user.PhoneNumber, code);
                
                var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, Input.OTP);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (token != null)
                        await _repository.ChangeTokenAsync(token);

                    StatusMessage = "Success : OTP Confirmed";
                    Input.OTP = "";
                    return Page();
                }

            }

            Input.OTP = "";
            StatusMessage = "Error : The OTP you entered could not be authenticated, Please try again.";
            return Page();
        }
    }
}
