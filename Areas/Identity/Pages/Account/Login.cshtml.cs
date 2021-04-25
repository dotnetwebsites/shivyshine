using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Shivyshine.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Shivyshine.Data;

namespace Shivyshine.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _repository;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _repository = repository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "Username/Mobile Number/Email Address")]
            [Required(ErrorMessage = "Please enter Username/Mobile Number/Email Address")]
            [RegularExpression(@"^\S*$", ErrorMessage = "Space not allowed, please enter valid Username/Mobile Number/Email Address")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Please enter password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                if (await _userManager.FindByNameAsync(Input.Username) != null)
                {
                    Input.Username = (await _userManager.FindByNameAsync(Input.Username)).UserName;
                }
                else if (await _userManager.FindByEmailAsync(Input.Username) != null)
                {
                    Input.Username = (await _userManager.FindByEmailAsync(Input.Username)).UserName;
                }
                else if (_userManager.Users.Any(p => p.PhoneNumber == Input.Username))
                {
                    Input.Username = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == Input.Username).UserName;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Input.Username + " user not found in our database, please sign up.");
                    return Page();
                }

                //to check if user is confirmed or not
                var user = await _userManager.FindByNameAsync(Input.Username);

                // This code is just for skipping email confirmation
                // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // await _userManager.ConfirmEmailAsync(user, code);

                if (user != null && !user.EmailConfirmed)
                {
                    return Redirect("/Account/SendEmailConfirmLink");
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    //bool isMobile = Request.Headers["User-Agent"].ToString().ToLower().Contains("mobile") ? true : false;

                    CookieOptions option = new CookieOptions();
                    PopulateMenus menus = new PopulateMenus(_repository, Input.Username);

                    Response.Cookies.Delete("menus");
                    Response.Cookies.Append("menus", menus.MenuText(), option);

                    //Response.Cookies.Append("slidermenus", menus.SliderMenuText(), option);
                    //Response.Cookies.Delete("slidermenus");

                    if (await _userManager.IsInRoleAsync(user, "admin"))
                    {
                        //return Redirect("/Admin/Home/Index");
                        return Redirect("/Home/Index");
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
