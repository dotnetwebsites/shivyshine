using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    //[Log]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _repository;

        public AccountController(ApplicationDbContext repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(EmployeeLoginModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            if (_repository.Employees.Any(p => p.Username == model.Username && p.Password == Encrypt.TextSHA512(model.Password)))
            {
                var emp = await _repository.Employees.FirstAsync(p => p.Username == model.Username && p.Password == Encrypt.TextSHA512(model.Password));

                if (!emp.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your ID has been discontinued");
                    return View(model);
                }


                CookieOptions option = new CookieOptions();
                Response.Cookies.Append("EMPCODE", emp.EmpCode, option);
                Response.Cookies.Append("NAME", emp.FullName, option);
                Response.Cookies.Append("MOBILE", emp.MobileNo, option);
                Response.Cookies.Append("USERNAME", emp.Username, option);
                Response.Cookies.Append("EMAIL", emp.Email, option);
                return RedirectToAction("", "", new { area = "Admin" });
            }
            else
                ModelState.AddModelError(string.Empty, "Invalid Username or Password");

            return View(model);
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Login", "Account", new { @area = "Admin" });
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            // var user = await _userManager.GetUserAsync(User);
            // if (user == null)
            // {
            //     return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            // }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (Request.Cookies["USERNAME"] == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var emp = await _repository.Employees.FirstAsync(p => p.Username == Request.Cookies["USERNAME"].ToString());

                if (emp.Password != Encrypt.TextSHA512(model.OldPassword))
                {
                    ModelState.AddModelError(string.Empty, "Current password is not valid.");
                    return View(model);
                }

                if (emp.Password == Encrypt.TextSHA512(model.OldPassword))
                {
                    emp.Password = Encrypt.TextSHA512(model.NewPassword);
                    await _repository.SaveChangesAsync();

                    ModelState.AddModelError(string.Empty, "Password successfully changed.");
                }

            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}