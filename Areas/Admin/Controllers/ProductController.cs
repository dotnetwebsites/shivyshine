using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public ProductController(ApplicationDbContext repository,
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
            var models = _mapper.Map<IEnumerable<ProductView>>(await _repository.Products.ToListAsync());
            try
            {
                foreach (var model in models)
                {
                    model.BrandName = (await _repository.Brands.FirstOrDefaultAsync(p => p.Id == model.BrandId)).BrandName;
                    model.CategoryName = (await _repository.Categories.FirstOrDefaultAsync(p => p.Id == model.CategoryId)).CategoryName;
                    model.SubCategoryName = (await _repository.SubCategories.FirstOrDefaultAsync(p => p.Id == model.SubCategoryId)).SubCategoryName;
                    model.SuperCategoryName = (await _repository.SuperCategories.FirstOrDefaultAsync(p => p.Id == model.SuperCategoryId)).SuperCategoryName;

                    //model.ProductImageUrl = (await _repository.ProductImages.FirstOrDefaultAsync(p => p.ProductId == model.Id && p.IsMainPic))?.ProductImageUrl;
                }

                return View(models);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(models);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName");
            ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName");
            //ViewBag.SubCategoryId = new SelectList(_repository.SubCategories, "Id", "SubCategoryName");

            ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "GM", Value = "GM" },
                new SelectListItem { Text = "ML", Value = "ML" }
            }, "Value", "Text");
            return View();
        }

        [HttpGet]
        public JsonResult LoadSubCategory(int id)
        {
            var subCategory = _repository.SubCategories.Where(p => p.CategoryId == id).ToList();
            return Json(new SelectList(subCategory, "Id", "SubCategoryName"));
        }

        [HttpGet]
        public JsonResult LoadSuperCategory(int id)
        {
            var superCategory = _repository.SuperCategories.Where(p => p.SubCategoryId == id).ToList();
            return Json(new SelectList(superCategory, "Id", "SuperCategoryName"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.SubCategoryId <= 0 || model?.SubCategoryId == null ||
                    model.SuperCategoryId <= 0 || model?.SuperCategoryId == null)
                    {
                        if (model.SuperCategoryId <= 0 || model?.SuperCategoryId == null)
                            ModelState.AddModelError(string.Empty, "Please select super category");

                        if (model.SubCategoryId <= 0 || model?.SubCategoryId == null)
                            ModelState.AddModelError(string.Empty, "Please select sub category");

                        ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", model.BrandId);
                        ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                        ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == model.CategoryId), "Id", "SubCategoryName", model.SubCategoryId);
                        ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == model.SubCategoryId), "Id", "SuperCategoryName", model.SuperCategoryId);

                        return View(model);
                    }

                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.Products.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    // if (model.ProductImage != null)
                    //     await UploadProductImages(model);

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", model.BrandId);
                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == model.CategoryId), "Id", "SubCategoryName", model.SubCategoryId);
                ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == model.SubCategoryId), "Id", "SuperCategoryName", model.SuperCategoryId);

                // ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                //     new SelectListItem { Text = "GM", Value = "GM" },
                //     new SelectListItem { Text = "ML", Value = "ML" }
                // }, "Value", "Text", model.QuantityType);


                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Products_HsnCode"))
                    ModelState.AddModelError(string.Empty, "HSN code already exist, please try another one.");
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Products_ProductName"))
                    ModelState.AddModelError(string.Empty, "Product name already exist, please try another one.");
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", model.BrandId);
                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == model.CategoryId), "Id", "SubCategoryName", model.SubCategoryId);
                ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == model.SubCategoryId), "Id", "SuperCategoryName", model.SuperCategoryId);

                // ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                //     new SelectListItem { Text = "GM", Value = "GM" },
                //     new SelectListItem { Text = "ML", Value = "ML" }
                // }, "Value", "Text", model.QuantityType);

                return View(model);
            }
        }

        // public async Task UploadProductImages(Product model)
        // {
        //     List<string> uniqueFiles = UploadedFiles(model);

        //     foreach (string FileName in uniqueFiles)
        //     {
        //         var isMainPic = await _repository.ProductImages.AnyAsync(p => p.ProductId == model.Id && p.IsMainPic);

        //         ProductImage productImage = new ProductImage
        //         {
        //             CreatedBy = User.Identity.Name,
        //             CreatedDate = DateTime.Now,
        //             IsMainPic = isMainPic ? false : true,
        //             ProductId = model.Id,
        //             ProductImageUrl = FileName
        //         };

        //         _repository.ProductImages.Add(productImage);
        //         await _repository.SaveChangesAsync();
        //     }
        // }

        // private List<string> UploadedFiles(Product model)
        // {
        //     List<string> list = new List<string>();

        //     string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

        //     if (!System.IO.Directory.Exists(uploadsFolder))
        //         System.IO.Directory.CreateDirectory(uploadsFolder);

        //     string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductImage.FileName;
        //     string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //     if (System.IO.File.Exists(Path.Combine(uploadsFolder, "TempZip.zip")))
        //         System.IO.File.Delete(Path.Combine(uploadsFolder, "TempZip.zip"));

        //     if (model.ProductImage != null)
        //     {
        //         string uploadszip = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

        //         filePath = Path.Combine(uploadszip, "TempZip.zip");

        //         using (var fileStream = new FileStream(filePath, FileMode.Create))
        //         {
        //             model.ProductImage.CopyTo(fileStream);
        //         }
        //     }

        //     using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(uploadsFolder, "TempZip.zip")))
        //     {
        //         foreach (ZipArchiveEntry entry in archive.Entries)
        //         {
        //             if (entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        //                 entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
        //                 entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        //             {

        //                 if (!System.IO.Directory.Exists(uploadsFolder + "/" + model.Id))
        //                     System.IO.Directory.CreateDirectory(uploadsFolder + "/" + model.Id);

        //                 // Gets the full path to ensure that relative segments are removed.entry.FullName
        //                 string destinationPath = Path.GetFullPath(Path.Combine(uploadsFolder + "/" + model.Id
        //                 , Guid.NewGuid().ToString() + ".jpg"));

        //                 list.Add(destinationPath.Replace("wwwroot", "~").Split('~')[1].ToString());

        //                 // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
        //                 // are case-insensitive.
        //                 if (destinationPath.StartsWith(uploadsFolder, StringComparison.Ordinal))
        //                     entry.ExtractToFile(destinationPath);
        //             }
        //         }
        //     }

        //     if (System.IO.File.Exists(Path.Combine(uploadsFolder, "TempZip.zip")))
        //         System.IO.File.Delete(Path.Combine(uploadsFolder, "TempZip.zip"));

        //     return list;
        // }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product model = await _repository.Products.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", model.BrandId);
            ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", model.CategoryId);
            ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == model.CategoryId), "Id", "SubCategoryName", model.SubCategoryId);
            ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == model.SubCategoryId), "Id", "SuperCategoryName", model.SuperCategoryId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (product.SubCategoryId <= 0 || product?.SubCategoryId == null ||
                    product.SuperCategoryId <= 0 || product?.SuperCategoryId == null)
                    {
                        if (product.SuperCategoryId <= 0 || product?.SuperCategoryId == null)
                            ModelState.AddModelError(string.Empty, "Please select super category");

                        if (product.SubCategoryId <= 0 || product?.SubCategoryId == null)
                            ModelState.AddModelError(string.Empty, "Please select sub category");

                        ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", product.BrandId);
                        ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", product.CategoryId);
                        ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == product.CategoryId), "Id", "SubCategoryName", product.SubCategoryId);
                        ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == product.SubCategoryId), "Id", "SuperCategoryName", product.SuperCategoryId);

                        return View(product);
                    }

                    product.UpdatedBy = User.Identity.Name;
                    product.UpdatedDate = DateTime.Now;

                    _repository.Entry(product).State = EntityState.Modified;
                    await _repository.SaveChangesAsync();

                    // if (product.ProductImage != null)
                    //     await UploadProductImages(product);

                    return RedirectToAction("Index");
                }

                ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", product.BrandId);
                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", product.CategoryId);
                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == product.CategoryId), "Id", "SubCategoryName", product.SubCategoryId);
                ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == product.SubCategoryId), "Id", "SuperCategoryName", product.SuperCategoryId);

                // ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                //     new SelectListItem { Text = "GM", Value = "GM" },
                //     new SelectListItem { Text = "ML", Value = "ML" }
                // }, "Value", "Text", product.QuantityType);

                //ViewBag.ProductImages = _repository.ProductImages.Where(p => p.ProductId == product.Id).ToList();
                //ViewBag.ProductImages
                return View(product);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Products_HsnCode"))
                    ModelState.AddModelError(string.Empty, "HSN code already exist, please try another one.");
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Products_ProductName"))
                    ModelState.AddModelError(string.Empty, "Product name already exist, please try another one.");
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.BrandId = new SelectList(_repository.Brands, "Id", "BrandName", product.BrandId);
                ViewBag.CategoryId = new SelectList(_repository.Categories, "Id", "CategoryName", product.CategoryId);
                ViewBag.SubCategoryId = new SelectList(_repository.SubCategories.Where(p => p.CategoryId == product.CategoryId), "Id", "SubCategoryName", product.SubCategoryId);
                ViewBag.SuperCategoryId = new SelectList(_repository.SuperCategories.Where(p => p.SubCategoryId == product.SubCategoryId), "Id", "SuperCategoryName", product.SuperCategoryId);

                // ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                //     new SelectListItem { Text = "GM", Value = "GM" },
                //     new SelectListItem { Text = "ML", Value = "ML" }
                // }, "Value", "Text", product.QuantityType);

                //ViewBag.ProductImages = _repository.ProductImages.Where(p => p.ProductId == product.Id).ToList();
                return View(product);
            }

        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Product product = await _repository.Products.FindAsync(id);

            var images = await _repository.ProductImages.Where(p => p.ProductId == product.Id).ToListAsync();

            foreach (var i in images)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "productimages/", i.ProductImageUrl);
                var productUnit = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.ProductId == product.Id);
                var shade = await _repository.Shades.FirstOrDefaultAsync(p => p.ProductId == product.Id);

                if (productUnit != null)
                {
                    _repository.ProductUnits.Remove(productUnit);
                    await _repository.SaveChangesAsync();
                }
                if (shade != null)
                {
                    _repository.Shades.Remove(shade);
                    await _repository.SaveChangesAsync();
                }

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _repository.ProductImages.Remove(i);
                await _repository.SaveChangesAsync();

            }

            _repository.Products.Remove(product);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportProduct>>(await _repository.Products.ToListAsync());

            foreach (var item in results)
            {
                var prod = await _repository.Products.FindAsync(item.Id);

                item.Brand = (await _repository.Brands.FindAsync(prod.BrandId)).BrandName;
                item.Category = (await _repository.Categories.FindAsync(prod.CategoryId)).CategoryName;
                item.SubCategory = (await _repository.SubCategories.FindAsync(prod.SubCategoryId)).SubCategoryName;
                item.SuperCategory = (await _repository.SuperCategories.FindAsync(prod.SuperCategoryId)).SuperCategoryName;
            }
            
            return _ex.Export<ExportProduct>(results, "Product", "Product");
        }

        [HttpGet]
        public IActionResult Demo()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProductImage(int id)
        {
            var productimage = await _repository.ProductImages.FindAsync(id);
            return View(productimage);
        }

    }
}