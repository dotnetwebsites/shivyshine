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
    public class SubBannerController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public SubBannerController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var models = _mapper.Map<IEnumerable<SubBannerView>>(await _repository.SubBanners.ToListAsync());

            foreach (var model in models)
            {
                model.SuperCategory = (await _repository.SuperCategories.FirstOrDefaultAsync(p => p.Id == model.SupCatId)).SuperCategoryName;
            }


            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubBanner model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.BannerImage == null)
                    {
                        ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
                        ModelState.AddModelError(string.Empty, "Please choose banner image");
                        return View(model);
                    }

                    string uniqueFiles = UploadedFile(model);

                    if (uniqueFiles == null)
                    {
                        ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
                        ModelState.AddModelError(string.Empty, "Invalid file");
                        return View(model);
                    }

                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.BannerUrl = uniqueFiles;

                    _repository.SubBanners.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
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
            SubBanner model = await _repository.SubBanners.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubBanner model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/SubBanner");
            string filePath = "";
            try
            {
                if (ModelState.IsValid)
                {
                    SubBanner c = _repository.SubBanners.Find(model.Id);

                    c.UpdatedBy = User.Identity.Name;
                    c.UpdatedDate = DateTime.Now;
                    c.BannerTitle = model.BannerTitle;
                    c.SupCatId = model.SupCatId;
                    c.RedirectedUrl = model.RedirectedUrl;

                    if (model.BannerImage != null)
                    {
                        if (c.BannerUrl != null)
                            filePath = Path.Combine(_webHostEnvironment.WebRootPath, "subbanner/", c.BannerUrl);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        string uniqueFiles = UploadedFile(model);

                        if (uniqueFiles == null)
                        {
                            ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
                            ModelState.AddModelError(string.Empty, "Invalid file");
                            return View(model);
                        }

                        c.BannerUrl = uniqueFiles;
                    }

                    await _repository.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }

                ViewBag.SupCatId = new SelectList(_repository.SuperCategories, "Id", "SuperCategoryName", model.SupCatId);
                return View(model);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);

            }
        }

        private string UploadedFile(SubBanner model)
        {
            string uniqueFileName = null;

            if (model.BannerImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "subbanner/");

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

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SubBanner banner = await _repository.SubBanners.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "subbanner/", banner.BannerUrl);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _repository.SubBanners.Remove(banner);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}