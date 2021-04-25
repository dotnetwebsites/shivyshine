using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Services;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMailService _emailSender;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger,
            IMailService emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter first name")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Please enter username")]
            [Display(Name = "Username")]
            [MinLength(5, ErrorMessage = "Minimum 5 characters required in username")]
            [RegularExpression(@"^\S*$", ErrorMessage = "Space not allowed, please enter valid username")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Required mobile no")]
            [MaxLength(10)]
            [MinLength(10, ErrorMessage = "Mobile no must be 10-digit without prefix")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile no must be numeric")]
            [Display(Name = "Mobile No")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Please enter email address")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password must required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Required Date of Birth")]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date, ErrorMessage = "Required Date of Birth")]
            [DateOfBirth(MinAge = 18, MaxAge = 150, ErrorMessage = "Eligibility above 18 years only.")]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Gender must required")]
            public bool Gender { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if ((await _userManager.FindByEmailAsync(Input.Email)) != null)
                {
                    ModelState.AddModelError(string.Empty, $"{ Input.Email } already taken, please use another and try again.");
                    return Page();
                }

                if (_userManager.Users.Any(p => p.PhoneNumber == Input.Email))
                {
                    ModelState.AddModelError(string.Empty, $"{ Input.Email } already taken, please use another and try again.");
                    return Page();
                }

                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstName = Input.Firstname,
                    LastName = Input.Lastname,
                    PhoneNumber = Input.PhoneNumber,
                    DateOfBirth = Input.DateOfBirth,
                    Gender = Input.Gender
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await this.CreateNewRoleAsync(user);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    MailContent mail = new MailContent("Shivyshine", "Please confirm your Email address",
                        HtmlEncoder.Default.Encode(callbackUrl), "Click here");

                    _emailSender.SendEmail(Mail.DNR, Input.Email, user.FullName, "Confirm your email", mail.Content);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task CreateNewRoleAsync(ApplicationUser user)
        {

            if (user.UserName == "admin")
            {
                IdentityRole adminRole = new IdentityRole
                {
                    Name = user.UserName
                };

                IdentityRole superAdminRole = new IdentityRole
                {
                    Name = "superadmin"
                };

                if (await _roleManager.FindByNameAsync(adminRole.Name) == null)
                    await _roleManager.CreateAsync(adminRole);

                if (await _roleManager.FindByNameAsync(superAdminRole.Name) == null)
                    await _roleManager.CreateAsync(superAdminRole);

                await _userManager.AddToRoleAsync(user, adminRole.Name);
                IdentityResult result = await _userManager.AddToRoleAsync(user, superAdminRole.Name);

                if (result.Succeeded)
                {
                    UserMenuRoleViewModel userMenuRoleViewModel = new UserMenuRoleViewModel();

                    int[] ids = new int[] { 2, 7, 8, 9, 10, 11, 12 };
                    var menus = _db.DynamicMenus.Where(p => ids.Contains(p.Id)).ToList();

                    var superadminrole = await _roleManager.FindByNameAsync(superAdminRole.Name);

                    foreach (var menu in menus)
                    {
                        if (!await userMenuRoleViewModel.IsInMenuRoleAsync(menu, superadminrole.Id, _db))
                            await userMenuRoleViewModel.AddToMenuRoleAsync(menu, superadminrole.Id, _db);
                    }

                }
            }

        }
    }
}
