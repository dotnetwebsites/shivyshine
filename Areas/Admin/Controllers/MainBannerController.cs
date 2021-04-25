using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class MainBannerController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public MainBannerController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var models = await _repository.MainBanners.ToListAsync();
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MainBanner model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.BannerImage == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please choose banner image");
                        return View(model);
                    }

                    string uniqueFiles = UploadedFile(model);

                    if (uniqueFiles == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid file");
                        return View(model);
                    }

                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.BannerUrl = uniqueFiles;
                    model.RedirectedUrl = model.RedirectedUrl;

                    _repository.MainBanners.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                return View(model);

            }
            catch (Exception ex)
            {
                var ban = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            MainBanner model = await _repository.MainBanners.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MainBanner model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/MainBanner");
            string filePath = "";
            try
            {
                if (ModelState.IsValid)
                {
                    MainBanner c = _repository.MainBanners.Find(model.Id);

                    c.UpdatedBy = User.Identity.Name;
                    c.UpdatedDate = DateTime.Now;
                    c.BannerTitle = model.BannerTitle;
                    c.RedirectedUrl = model.RedirectedUrl;

                    if (model.BannerImage != null)
                    {
                        if (c.BannerUrl != null)
                            filePath = Path.Combine(_webHostEnvironment.WebRootPath, "mainbanner/", c.BannerUrl);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        string uniqueFiles = UploadedFile(model);

                        if (uniqueFiles == null)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid file");
                            return View(model);
                        }

                        c.BannerUrl = uniqueFiles;
                    }

                    await _repository.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);

            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MainBanner banner = await _repository.MainBanners.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "mainbanner/", banner.BannerUrl);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _repository.MainBanners.Remove(banner);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

        private string UploadedFile(MainBanner model)
        {
            string uniqueFileName = null;

            if (model.BannerImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "mainbanner/");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BannerImage.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.BannerImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

    }
}