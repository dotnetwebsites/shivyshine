using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Shivyshine.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Shivyshine.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Shivyshine.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class ChangePhoneNumberModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePhoneNumberModel> _logger;
        private readonly ILogToken _repository;

        private CookieOptions option = new CookieOptions();

        public ChangePhoneNumberModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePhoneNumberModel> logger,
            ILogToken repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _repository = repository;
        }

        public string Username { get; set; }

        [Required(ErrorMessage = "Required mobile no")]
        [MaxLength(10)]
        [MinLength(10, ErrorMessage = "10-digit mobile number withour prefixes")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
        [Display(Name = "Mobile No")]
        public string PhoneNumber { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New Mobile No")]
            public string NewPhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var phonenumber = await _userManager.GetPhoneNumberAsync(user);
            PhoneNumber = phonenumber;

            Input = new InputModel
            {
                NewPhoneNumber = phonenumber,
            };

            IsPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostChangePhoneNumberAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phonenumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.NewPhoneNumber != phonenumber)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, Input.NewPhoneNumber);
                if (result.Succeeded)
                {
                    //StatusMessage = "Verification sms has been sent on mobile. Please check your message.";
                    StatusMessage = "Mobile no changed successfully";
                }
                else
                    StatusMessage = "Something went wrong.";

                return RedirectToPage();
            }

            StatusMessage = "Warning : Your number is unchanged.";
            return RedirectToPage();
        }
        // Response.Cookies.Append("isTimer", "1", option);

        public async Task<IActionResult> OnPostSendVerificationPhoneNumberAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            int tokenid = await _repository.IsLogTokenExpiredAsync(userId, phoneNumber);

            var callbackUrl = Url.Page(
                "/Account/ConfirmPhoneNumberChange",
                pageHandler: null,
                values: new { userId = userId, phonenumber = phoneNumber },
                protocol: Request.Scheme);

            //if token already exists
            if (tokenid > 0)
            {
                return Redirect(callbackUrl);
            }

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            _logger.LogInformation(code);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var result = await _repository.GenerateLogTokenAsync(TokenType.PHONE, userId, phoneNumber, code);

            if (result == RepositoryResult.Succeeded)
                return Redirect(callbackUrl);
            else
                return Page();

        }

        public async Task<IActionResult> OnPostSendCancelPhone()
        {
            var user = await _userManager.GetUserAsync(User);

            Response.Cookies.Delete("isTimer");
            Response.Cookies.Delete("minCount");

            return RedirectToAction("UserProfile", "Account");
        }
    }
}


//_logger.LogInformation(code);
// return RedirectToAction("ConfirmPhoneNumberChange", "Account",
// routeValues: new { userId = userId, phonenumber = phoneNumber, code = code });