using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Services;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class RegisteredUsersController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailService _emailSender;

        public RegisteredUsersController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper,
                                IMailService emailSender,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(string id = null)
        {
            if (id == null)
            {
                return NoContent();
            }

            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditEmployee(ApplicationUser model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                if (model.Email == "" || model.Email == null)
                {
                    ModelState.AddModelError(string.Empty, "Please enter email address.");
                    return View(model);
                }

                if (user != null)
                {
                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        user.EmailConfirmed = true;
                        var setUserNameResult = await _userManager.SetUserNameAsync(user, user.Email);
                        if (!setUserNameResult.Succeeded)
                        {
                            return View(model);
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, "Abc123#$%");

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        // var callbackUrl = Url.Action("ConfirmEmail", "Action",
                        // values: new { userId = userId, email = user.Email, code = code },
                        // protocol: Request.Scheme);                        
                        var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                        string htmlBody = @"<div style='font-family: Segoe UI;font-size: small;'>
                                            <strong>Dear " + user.FullName + @",</strong>
                                            <p>Greeting from Focus HRMS.</p>
                                            <p>Welcome to FOCUS Employee Self Service web portal that you can signin accesses with your </p>
                                            <p>Email ID: " + user.Email + @"</p>
                                            <p>Password: Abc123#$%</p>
                                            <p>Please click on URL: <a href='http://www.focushrms.com/'>http://www.focushrms.com/</a></p>
                                            <p>Once you log in, you can access employee information.</p>
                                            <p><strong>Regards,</strong></p>
                                            <p><strong>Focus HR Solutions</strong></p>
                                            </div>";

                        // string body = "<p>Dear " + user.FullName + $@"</p>
                        //             <p>Your Password is : Abc123#$%</p><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</<p>";

                        _emailSender.SendEmail(
                            Mail.DNR,
                            user.Email,
                            user.FullName,
                            "Email address has been updated", htmlBody);

                    }

                    // user.EmployeeCode = model.EmployeeCode;

                    user.PhoneNumber = model.PhoneNumber;
                    // user.Designation = model.Designation;
                    // user.DateofJoining = model.DateofJoining;
                    // user.Department = model.Department;
                    // user.WorkLocation = model.WorkLocation;
                    // user.IsActive = model.IsActive;

                    //string result = await ChangeEmail(user, model);
                    //await _signInManager.RefreshSignInAsync(user);
                    await _repository.SaveChangesAsync();
                }

                return RedirectToAction("ManageEmployee", _userManager.Users);
            }

            return View(model);
        }

    }
}


