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
    public class ProductUnitController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public ProductUnitController(ApplicationDbContext repository,
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
            var productUnits = await _repository.ProductUnits.ToListAsync();

            foreach (var prod in productUnits)
            {
                var p = await _repository.Products.FindAsync(prod.ProductId);
                var b = await _repository.Brands.FindAsync(p.BrandId);

                prod.ProductName = b.BrandName + " - " + p.ProductName;

                prod.ProductUnitImageUrl =
                _repository.ProductImages.FirstOrDefault(p => p.ProductId == prod.ProductId &&
                p.ProductUnitId == prod.Id)?.ProductImageUrl;
            }

            return View(productUnits);
        }

        [HttpGet]
        public JsonResult LoadProducts(int id)
        {
            var products = _repository.Products.Where(p => p.BrandId == id).ToList();
            return Json(new SelectList(products.OrderBy(p => p.ProductName), "Id", "ProductName"));
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "GM", Value = "GM" },
                    new SelectListItem { Text = "ML", Value = "ML" },
                    new SelectListItem { Text = "PCS", Value = "PCS" }
                }, "Value", "Text");


            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName");
            //ViewBag.ProductId = new SelectList(_repository.Products, "Id", "ProductName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductUnit model)
        {
            if (ModelState.IsValid)
            {
                if (model?.ProductId == null || model.ProductId < 1)
                {
                    ModelState.AddModelError(string.Empty, "Please select product and try again");

                    ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                                            new SelectListItem { Text = "GM", Value = "GM" },
                                            new SelectListItem { Text = "ML", Value = "ML" },
                                            new SelectListItem { Text = "PCS", Value = "PCS" }
                                            }, "Value", "Text", model.QuantityType);

                    ViewBag.ProductId = new SelectList(_repository.Products, "Id", "ProductName", model.ProductId);

                    return View(model);
                }

                model.CreatedBy = User.Identity.Name;
                model.CreatedDate = DateTime.Now;

                _repository.ProductUnits.Add(model);
                int result = await _repository.SaveChangesAsync();

                if (model.ProductUnitImages != null)
                    await UploadProductImages(model);

                return RedirectToAction("Index", null, new { area = "Admin" });
            }

            ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "GM", Value = "GM" },
                    new SelectListItem { Text = "ML", Value = "ML" },
                    new SelectListItem { Text = "PCS", Value = "PCS" }
                }, "Value", "Text", model.QuantityType);

            ViewBag.ProductId = new SelectList(_repository.Products, "Id", "ProductName", model.ProductId);

            return View(model);
        }

        public async Task UploadProductImages(ProductUnit model)
        {
            List<string> uniqueFiles = UploadedFiles(model);

            foreach (string FileName in uniqueFiles)
            {
                var isMainPic = await _repository.ProductImages.AnyAsync(p => p.ProductId == model.Id && p.IsMainPic);

                ProductImage productImage = new ProductImage
                {
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsMainPic = isMainPic ? false : true,
                    ProductId = model.ProductId,
                    ProductImageUrl = FileName,
                    ProductUnitId = model.Id
                };

                _repository.ProductImages.Add(productImage);
                await _repository.SaveChangesAsync();
            }
        }

        private List<string> UploadedFiles(ProductUnit model)
        {
            List<string> list = new List<string>();

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

            if (!System.IO.Directory.Exists(uploadsFolder))
                System.IO.Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProductUnitImages.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (System.IO.File.Exists(Path.Combine(uploadsFolder, "TempZip.zip")))
                System.IO.File.Delete(Path.Combine(uploadsFolder, "TempZip.zip"));

            if (model.ProductUnitImages != null)
            {
                string uploadszip = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

                filePath = Path.Combine(uploadszip, "TempZip.zip");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProductUnitImages.CopyTo(fileStream);
                }
            }

            using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(uploadsFolder, "TempZip.zip")))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {

                        if (!System.IO.Directory.Exists(uploadsFolder + "/" + model.Id))
                            System.IO.Directory.CreateDirectory(uploadsFolder + "/" + model.Id);

                        // Gets the full path to ensure that relative segments are removed.entry.FullName
                        string destinationPath = Path.GetFullPath(Path.Combine(uploadsFolder + "/" + model.Id
                        , Guid.NewGuid().ToString() + ".jpg"));

                        list.Add(destinationPath.Replace("wwwroot", "~").Split('~')[1].ToString());

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(uploadsFolder, StringComparison.Ordinal))
                            entry.ExtractToFile(destinationPath);
                    }
                }
            }

            if (System.IO.File.Exists(Path.Combine(uploadsFolder, "TempZip.zip")))
                System.IO.File.Delete(Path.Combine(uploadsFolder, "TempZip.zip"));

            return list;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductUnit model = await _repository.ProductUnits.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = new SelectList(_repository.Products.OrderBy(p => p.ProductName), "Id", "ProductName", model.ProductId);
            var prod = await _repository.Products.FindAsync(model.ProductId);
            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName", prod.BrandId);

            ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "GM", Value = "GM" },
                    new SelectListItem { Text = "ML", Value = "ML" },
                    new SelectListItem { Text = "PCS", Value = "PCS" }
                }, "Value", "Text", model.QuantityType);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUnit product)
        {
            if (ModelState.IsValid)
            {
                if (product?.ProductId == null)
                {
                    ModelState.AddModelError(string.Empty, "Please select product and try again");

                    ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                                            new SelectListItem { Text = "GM", Value = "GM" },
                                            new SelectListItem { Text = "ML", Value = "ML" },
                                            new SelectListItem { Text = "PCS", Value = "PCS" }
                                            }, "Value", "Text", product.QuantityType);

                    ViewBag.ProductId = new SelectList(_repository.Products, "Id", "ProductName", product.ProductId);
                    var pro = await _repository.Products.FindAsync(product.ProductId);
                    ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName", pro.BrandId);
                    return View(product);
                }

                product.UpdatedBy = User.Identity.Name;
                product.UpdatedDate = DateTime.Now;

                _repository.Entry(product).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                if (product.ProductUnitImages != null)
                    await UploadProductImages(product);

                return RedirectToAction("Index");
            }

            // var prods = _repository.Products.ToList();
            // foreach (var prod in prods)
            // {
            //     var brand = await _repository.Brands.FindAsync(prod.BrandId);
            //     prod.ProductName = brand.BrandName + " - " + prod.ProductName;
            // }

            // ViewBag.ProductId = new SelectList(prods, "Id", "ProductName", product.ProductId);

            ViewBag.ProductId = new SelectList(_repository.Products.OrderBy(p => p.ProductName), "Id", "ProductName", product.ProductId);
            var prod = await _repository.Products.FindAsync(product.ProductId);
            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName", prod.BrandId);

            ViewBag.QuantityType = new SelectList(new List<SelectListItem> {
                    new SelectListItem { Text = "GM", Value = "GM" },
                    new SelectListItem { Text = "ML", Value = "ML" },
                    new SelectListItem { Text = "PCS", Value = "PCS" }
                }, "Value", "Text", product.QuantityType);

            //ViewBag.ProductImages = _repository.ProductImages.Where(p => p.ProductId == product.Id).ToList();

            return View(product);

        }

        [Authorize(Roles = "superadmin")]
        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            ProductUnit productUnit = await _repository.ProductUnits.FindAsync(id);

            var images = _repository.ProductImages.Where(p => p.ProductId == productUnit.ProductId && p.ProductUnitId == id).ToList();
            foreach (var img in images)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "productimages/", img.ProductImageUrl);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _repository.ProductImages.Remove(img);
                await _repository.SaveChangesAsync();
            }

            _repository.ProductUnits.Remove(productUnit);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportProductUnit>>(await _repository.ProductUnits.ToListAsync());

            foreach (var item in results)
            {
                var unit = await _repository.ProductUnits.FindAsync(item.Id);

                item.ProductName = (await _repository.Products.FindAsync(unit.ProductId)).ProductName;
            }

            return _ex.Export<ExportProductUnit>(results, "ProductUnit", "ProductUnit");
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