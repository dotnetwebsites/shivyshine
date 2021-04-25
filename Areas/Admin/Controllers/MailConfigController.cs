using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Services;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [Area("Admin")]
    [AllowAnonymous]
    public class MailConfigController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMailService _emailSender;

        public MailConfigController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMailService emailSender)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var mailLibraries = await _repository.MailLibraries.ToListAsync();
            return View(mailLibraries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MailLibrary model)
        {
            if (ModelState.IsValid)
            {
                _repository.MailLibraries.Add(model);
                int result = await _repository.SaveChangesAsync();

                return RedirectToAction("Index", null, new { area = "Admin" });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MailLibrary mailLibrary = await _repository.MailLibraries.FindAsync(id);
            if (mailLibrary == null)
            {
                return NotFound();
            }

            return View(mailLibrary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MailLibrary mailLibrary)
        {
            if (ModelState.IsValid)
            {
                _repository.Entry(mailLibrary).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(mailLibrary);
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            MailLibrary mailLibrary = await _repository.MailLibraries.FindAsync(id);

            _repository.MailLibraries.Remove(mailLibrary);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

        [HttpGet]
        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMail(MailLibraryView model)
        {
            if (ModelState.IsValid)
            {
                _emailSender.SendEmail(model.MailType, model.EmailAddress, model.Username,
                model.Subject, model.Content);

                return RedirectToAction("Index", null, new { area = "Admin" });
            }

            return View(model);
        }

    }
}