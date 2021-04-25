using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Services;
using Shivyshine.Utilities;

namespace Shivyshine.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ApplicationDbContext db;

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMailService _emailSender;
        private readonly ILogToken _repository;
        private readonly IMapper _mapper;



        public AccountController(SignInManager<ApplicationUser> signInManager,
                                UserManager<ApplicationUser> userManager,
                                ApplicationDbContext dbContext,
                                IWebHostEnvironment hostEnvironment,
                                IMailService emailSender,
                                ILogToken repository,
                                IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            db = dbContext;
            webHostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            else
            {
                //Sessions.Clear(Response, _Cache);
                return Redirect("/Identity/Account/Login");
            }

        }

        public LoginModel Input { get; set; }

        public class LoginModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> UserLogin(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok("loggedin");
                }
                else
                    return NotFound("not found");

            }

            return NotFound(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UserLogout()
        {
            await _signInManager.SignOutAsync();
            return Ok("loggedout");
        }


        [Authorize]
        [HttpGet]
        public IActionResult CheckConnection()
        {
            return Ok("db.Products");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            string path = Path.Combine(webHostEnvironment.WebRootPath, "/assets/img/avatar/");

            if (!System.IO.File.Exists(webHostEnvironment.WebRootPath + user.ProfileImage))
            {
                user.ProfileImage = path + (user.Gender ? "male_avatar.png" : "female_avatar.png");
            }

            return View(user);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonalInfo(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            string path = Path.Combine(webHostEnvironment.WebRootPath, "/assets/img/avatar/");

            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (!System.IO.Directory.Exists(webHostEnvironment.WebRootPath + user.ProfileImage))
                        System.IO.File.Delete(webHostEnvironment.WebRootPath + user.ProfileImage);

                    string imageUrl = model.Avatar == null ? "" : UploadedFile(model);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Gender = model.Gender;

                    if (imageUrl != "")
                        user.ProfileImage = imageUrl;

                    user.DateOfBirth = model.DateOfBirth;

                    await db.SaveChangesAsync();
                }

                if (!System.IO.File.Exists(webHostEnvironment.WebRootPath + user.ProfileImage))
                {
                    user.ProfileImage = path + (user.Gender ? "male_avatar.png" : "female_avatar.png");
                }

                return View(user);

                //return RedirectToAction("PersonalInfo");
            }

            if (!System.IO.File.Exists(webHostEnvironment.WebRootPath + user.ProfileImage))
            {
                user.ProfileImage = path + (user.Gender ? "male_avatar.png" : "female_avatar.png");
            }

            return View(user);
        }

        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = null;

            if (model.Avatar != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "users/avatar");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Avatar.CopyTo(fileStream);
                }
            }
            return "/users/avatar/" + uniqueFileName;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SendEmailConfirmLink()
        {
            if (User.Identity.Name != null)
            {
                return Redirect("/");
            }

            SendEmailConfirmLinkViewModel model = new SendEmailConfirmLinkViewModel();

            model.IsSuccess = false;
            ModelState.AddModelError(string.Empty, $"Your email address not confirmed yet!");

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendEmailConfirmLink(SendEmailConfirmLinkViewModel model)
        {
            if (ModelState.IsValid)
            {
                var findByName = await _userManager.FindByNameAsync(model.Value);
                var findByEmail = await _userManager.FindByEmailAsync(model.Value);

                if (findByName != null)
                {
                    if (findByName.EmailConfirmed)
                    {
                        model.IsSuccess = true;
                        ModelState.AddModelError(string.Empty, $"Email already confirmed");
                        return View(model);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(findByName);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = findByName.Id, code = code },
                    protocol: Request.Scheme);

                    MailContent mail = new MailContent("Shivyshine", "Please confirm your Email address", HtmlEncoder.Default.Encode(callbackUrl), "Click here");

                    _emailSender.SendEmail(findByName.Email, findByName.FullName, "Confirm your email", mail.Content);

                    var result = await _repository.GenerateLogTokenAsync(TokenType.EMAIL, findByName.Id, findByName.Email, code);

                    model.Value = "";
                    model.IsSuccess = true;
                    ModelState.AddModelError(string.Empty, $"Confirmation link has been sent, please check your mailbox.");
                    return View(model);

                }
                else if (findByEmail != null)
                {
                    if (findByEmail.EmailConfirmed)
                    {
                        model.IsSuccess = true;
                        ModelState.AddModelError(string.Empty, $"Email already confirmed");
                        return View(model);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(findByEmail);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = findByEmail.Id, code = code },
                    protocol: Request.Scheme);

                    MailContent mail = new MailContent("Samyra Global", "Please confirm your Email address", HtmlEncoder.Default.Encode(callbackUrl), "Click here");

                    _emailSender.SendEmail(findByEmail.Email, findByEmail.FullName, "Confirm your email", mail.Content);

                    var result = await _repository.GenerateLogTokenAsync(TokenType.EMAIL, findByEmail.Id, findByEmail.Email, code);

                    model.Value = "";
                    model.IsSuccess = true;
                    ModelState.AddModelError(string.Empty, $"Confirmation link has been sent, please check your mailbox.");
                    return View(model);
                }
                else
                {
                    model.IsSuccess = false;
                    ModelState.AddModelError(string.Empty, $"The '{model.Value}' not found in database, please enter valid username or email address.");
                    return View(model);
                }

            }

            model.IsSuccess = false;
            ModelState.AddModelError(string.Empty, $"Your email address not confirmed yet!");
            return View(model);
        }

        public IActionResult MailBody()
        {
            StringBuilder sb = new StringBuilder();

            var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = User.Identity.Name, code = User.Identity.Name },
                            protocol: Request.Scheme);

            MailContent mail = new MailContent("Samyra Global", "Please confirm your Email address", callbackUrl, "Click here");

            ViewBag.html = sb.Append(mail.Content);

            //_emailSender.SendEmail("samyraglobal@gmail.com", "Sam Admin", "Confirm your email", sb.ToString());

            return View();
        }

    }
}