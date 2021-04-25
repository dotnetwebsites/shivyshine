using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _repository.Employees.ToListAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var isEmail = _repository.Employees.Any(p => p.Email == model.Email);
                    var isUser = _repository.Employees.Any(p => p.Username == model.Username);

                    if (isEmail || isUser)
                    {
                        if (isEmail)
                            ModelState.AddModelError(string.Empty, "Email address already exists.");

                        if (isUser)
                            ModelState.AddModelError(string.Empty, "Username already exists.");

                        return View(model);
                    }


                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.Password = Encrypt.TextSHA512("Shivy@123");

                    _repository.Employees.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                return View(model);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee model = await _repository.Employees.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/Employee");
            try
            {
                if (ModelState.IsValid)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;

                    _repository.Entry(model).State = EntityState.Modified;
                    await _repository.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<ActionResult> ResetPassword(int id)
        {
            var emp = await _repository.Employees.FindAsync(id);
            emp.Password = Encrypt.TextSHA512("Shivy@123");
            await _repository.SaveChangesAsync();
            var employees = await _repository.Employees.ToListAsync();
            ModelState.AddModelError(string.Empty, "Password has been successfully changed.");
            return View("Index", employees);
        }

        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Employee country = await _repository.Employees.FindAsync(id);

            _repository.Employees.Remove(country);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}