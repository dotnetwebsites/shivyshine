using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class SuperCategoryController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public SuperCategoryController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper, IExcel ex)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _ex = ex;
        }

        public async Task<IActionResult> Index()
        {
            var models = _mapper.Map<IEnumerable<SuperCategoryView>>(await _repository.SuperCategories.ToListAsync());

            foreach (var model in models)
            {
                var subcat = _repository.SubCategories.FirstOrDefault(p => p.Id == model.SubCategoryId);

                if (_repository.SubCategories.Any(p => p.Id == model.SubCategoryId))
                    model.SubCategoryName = _repository.SubCategories.FirstOrDefault(p => p.Id == model.SubCategoryId).SubCategoryName;
                else
                    model.SubCategoryName = "";

                if (subcat != null)
                {
                    if (_repository.Categories.Any(p => p.Id == subcat.CategoryId))
                        model.CategoryName = _repository.Categories.FirstOrDefault(p => p.Id == subcat.CategoryId).CategoryName;
                    else
                        model.CategoryName = "";
                }

                model.ImageUrl = model.ImageUrl != null ? "/supercategory/" + model.ImageUrl
                : "/assets/images/default_category.jpg";
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SuperCategory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFiles = UploadedFile(model);

                    if (uniqueFiles == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid file");
                        ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName", model.SubCategoryId);
                        return View(model);
                    }

                    SuperCategory superCategory = new SuperCategory
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        CategoryId = (await _repository.SubCategories.FirstOrDefaultAsync(p => p.Id == model.SubCategoryId)).CategoryId,
                        SubCategoryId = model.SubCategoryId,
                        SuperCategoryName = model.SuperCategoryName,
                        IsDisplay = model.IsDisplay,

                        ImageUrl = uniqueFiles
                    };

                    _repository.SuperCategories.Add(superCategory);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName", model.SubCategoryId);

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_SuperCategories_SuperCategoryName"))
                    ModelState.AddModelError(string.Empty, "Super category already exist with this name, please try another one.");

                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName", model.SubCategoryId);
                return View(model);
            }
        }

        private string UploadedFile(SuperCategory model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "supercategory/");

                if (!System.IO.Directory.Exists(uploadsFolder))
                    System.IO.Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SuperCategory model = await _repository.SuperCategories.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName", model.SubCategoryId);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SuperCategory model)
        {
            string filePath = "";
            try
            {
                if (ModelState.IsValid)
                {
                    SuperCategory c = _repository.SuperCategories.Find(model.Id);

                    c.UpdatedBy = User.Identity.Name;
                    c.UpdatedDate = DateTime.Now;

                    c.SubCategoryId = model.SubCategoryId;
                    c.CategoryId = (await _repository.SubCategories.FirstOrDefaultAsync(p => p.Id == model.SubCategoryId)).CategoryId;

                    c.SuperCategoryName = model.SuperCategoryName;
                    c.IsDisplay = model.IsDisplay;

                    if (model.ImageFile != null)
                    {
                        filePath = Path.Combine(_webHostEnvironment.WebRootPath, "supercategory/", c.ImageUrl);

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        string uniqueFiles = UploadedFile(model);

                        if (uniqueFiles == null)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid file");
                            return View(model);
                        }

                        c.ImageUrl = uniqueFiles;
                    }

                    //_repository.Entry(model).State = EntityState.Modified;
                    await _repository.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                return View(model);


            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_SuperCategories_SuperCategoryName"))
                    ModelState.AddModelError(string.Empty, "Super category already exist with this name, please try another one.");
                if (ex.Message.Contains("path3"))
                    ModelState.AddModelError(string.Empty, ex.Message + " " + filePath);
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName", model.SubCategoryId);
                return View(model);

            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportSuperCategory>>(await _repository.SuperCategories.ToListAsync());

            foreach (var item in results)
            {
                var supercat = await _repository.SuperCategories.FindAsync(item.Id);
                item.CategoryName = (await _repository.Categories.FindAsync(supercat.CategoryId)).CategoryName;
                item.SubCategoryName = (await _repository.SubCategories.FindAsync(supercat.SubCategoryId)).SubCategoryName;
            }

            return _ex.Export<ExportSuperCategory>(results, "Supercategory", "Supercategory");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            SuperCategory superCategory = await _repository.SuperCategories.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "supercategory/", superCategory.ImageUrl);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _repository.SuperCategories.Remove(superCategory);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}