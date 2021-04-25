using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Models;

namespace Shivyshine.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogToken _repository;
        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, ILogToken repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public string Title { get; set; }
        public bool AlertType { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            bool emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (emailConfirmed)
            {
                StatusMessage = "Error : Email already confirmed";
                return Page();
            }

            var token = await _repository.GetLogTokenAsync(user.Id, user.Email, code);

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                if (token != null)
                    await _repository.ChangeTokenAsync(token);

                Title = "Hurray!";
                StatusMessage = "You account has been successfully confirmed.";
                AlertType = true;
                return Page();
            }

            StatusMessage = "Error confirming your email.";
            Title = "Oops!";
            AlertType = false;
            return Page();
        }
    }
}
