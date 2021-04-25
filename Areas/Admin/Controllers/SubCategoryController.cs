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
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;
        public SubCategoryController(ApplicationDbContext repository,
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
            var models = _mapper.Map<IEnumerable<SubCategoryView>>(await _repository.SubCategories.ToListAsync());

            foreach (var model in models)
            {
                if (_repository.Categories.Any(p => p.Id == model.CategoryId))
                    model.CategoryName = _repository.Categories.FirstOrDefault(p => p.Id == model.CategoryId).CategoryName;
                else
                    model.CategoryName = "";

                model.ImageUrl = model.ImageUrl != null ? "/subcategory/" + model.ImageUrl
                : "/assets/images/default_category.jpg";
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFiles = UploadedFile(model);

                    if (uniqueFiles == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid file");
                        ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                        return View(model);
                    }

                    SubCategory subCategory = new SubCategory
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        CategoryId = model.CategoryId,
                        SubCategoryName = model.SubCategoryName,

                        ImageUrl = uniqueFiles
                    };

                    _repository.SubCategories.Add(subCategory);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_SubCategories_SubCategoryName"))
                    ModelState.AddModelError(string.Empty, "Sub category already exist with this name, please try another one.");

                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                return View(model);
            }
        }

        private string UploadedFile(SubCategory model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "subcategory/");

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
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            SubCategory model = await _repository.SubCategories.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategory model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/SubCategory");

            string filePath = "";
            try
            {
                if (ModelState.IsValid)
                {
                    SubCategory c = _repository.SubCategories.Find(model.Id);

                    c.UpdatedBy = User.Identity.Name;
                    c.UpdatedDate = DateTime.Now;
                    c.CategoryId = model.CategoryId;
                    c.SubCategoryName = model.SubCategoryName;

                    if (model.ImageFile != null)
                    {
                        filePath = Path.Combine(_webHostEnvironment.WebRootPath, "subcategory/", c.ImageUrl);

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

                    return LocalRedirect(returnUrl);
                    //return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_SubCategories_SubCategoryName"))
                    ModelState.AddModelError(string.Empty, "Sub category already exist with this name, please try another one.");
                if (ex.Message.Contains("path3"))
                    ModelState.AddModelError(string.Empty, ex.Message + " " + filePath);
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportSubCategory>>(await _repository.SubCategories.ToListAsync());

            foreach (var item in results)
            {
                var subcat = await _repository.SubCategories.FindAsync(item.Id);
                item.CategoryName = (await _repository.Categories.FindAsync(subcat.CategoryId)).CategoryName;
            }

            return _ex.Export<ExportSubCategory>(results, "Subcategory", "Subcategory");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            SubCategory subCategory = await _repository.SubCategories.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "subcategory/", subCategory.ImageUrl);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _repository.SubCategories.Remove(subCategory);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}