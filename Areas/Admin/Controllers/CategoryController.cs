using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IExcel _ex;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IExcel ex, IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _ex = ex;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _repository.Categories.ToListAsync();

            foreach (var category in categories)
            {
                category.ImageUrl = category.ImageUrl != null ? "/category/" + category.ImageUrl
                : "/assets/images/default_category.jpg";
            }

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Error();
            }
            Category model = await _repository.Categories.FindAsync(id);
            ViewBag.Img = Path.Combine(_webHostEnvironment.WebRootPath, "category/", model.ImageUrl);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.ImageFile == null)
                    {
                        ModelState.AddModelError(string.Empty, "Please choose category image");
                        return View(model);
                    }

                    string uniqueFiles = UploadedFile(model);

                    if (uniqueFiles == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid file");
                        return View(model);
                    }

                    Category category = new Category
                    {
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        CategoryName = model.CategoryName,

                        ImageUrl = uniqueFiles
                    };

                    _repository.Categories.Add(category);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Categories_CategoryName"))
                    ModelState.AddModelError(string.Empty, "Category already exist with this name, please try another one.");

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
            Category model = await _repository.Categories.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/Category");
            string filePath = "";
            try
            {

                if (ModelState.IsValid)
                {
                    Category c = _repository.Categories.Find(model.Id);

                    c.UpdatedBy = User.Identity.Name;
                    c.UpdatedDate = DateTime.Now;
                    c.CategoryName = model.CategoryName;

                    if (model.ImageFile != null)
                    {
                        filePath = Path.Combine(_webHostEnvironment.WebRootPath, "category/", c.ImageUrl);

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

                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Categories_CategoryName"))
                    ModelState.AddModelError(string.Empty, "Category already exist with this name, please try another one.");
                if (ex.Message.Contains("path3"))
                    ModelState.AddModelError(string.Empty, ex.Message + " " + filePath);
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);

            }
        }

        private string UploadedFile(Category model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "category/");

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
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportCategory>>(await _repository.Categories.ToListAsync());
            return _ex.Export<ExportCategory>(results, "Category", "Category");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Category category = await _repository.Categories.FindAsync(id);

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "category/", category.ImageUrl);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _repository.Categories.Remove(category);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}