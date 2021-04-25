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
    public class ShadeController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public ShadeController(ApplicationDbContext repository,
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
            var shades = await _repository.Shades.ToListAsync();

            foreach (var shade in shades)
            {
                var p = await _repository.Products.FindAsync(shade.ProductId);
                var b = await _repository.Brands.FindAsync(p.BrandId);

                shade.ProductName = b.BrandName + " - " + p.ProductName;

                shade.ProductShadeImageUrl =
                _repository.ProductImages.FirstOrDefault(p => p.ProductId == shade.ProductId &&
                p.ShadeId == shade.Id)?.ProductImageUrl;
            }

            return View(shades);
        }

        [HttpGet]
        public JsonResult LoadProducts(int id)
        {
            var products = _repository.Products.Where(p => p.BrandId == id).ToList();
            return Json(new SelectList(products.OrderBy(p => p.ProductName), "Id", "ProductName"));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var prods = _repository.Products.ToList();
            foreach (var prod in prods)
            {
                var brand = await _repository.Brands.FindAsync(prod.BrandId);
                prod.ProductName = brand.BrandName + " - " + prod.ProductName;
            }

            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName");
            //ViewBag.ProductId = new SelectList(prods, "Id", "ProductName");

            return View();
        }

        [HttpGet]
        public JsonResult ProductUnits(int id)
        {
            var productUnits = _repository.ProductUnits.Where(p => p.ProductId == id).ToList();
            foreach (var item in productUnits)
            {
                item.QuantityType = item.Quantity + " " + item.QuantityType;
            }
            return Json(new SelectList(productUnits, "Id", "QuantityType"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shade model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProductUnitId <= 0 ||
                    model?.ProductUnitId == null || model?.ProductId == null)
                {
                    if (model?.ProductId == null)
                        ModelState.AddModelError(string.Empty, "Please select product unit");
                    if (model.ProductUnitId <= 0 || model?.ProductUnitId == null)
                        ModelState.AddModelError(string.Empty, "Please select product unit");

                    var products = _repository.Products.ToList();
                    foreach (var prod in products)
                    {
                        var brand = await _repository.Brands.FindAsync(prod.BrandId);
                        prod.ProductName = brand.BrandName + " - " + prod.ProductName;
                    }

                    ViewBag.ProductId = new SelectList(products, "Id", "ProductName", model.ProductId);

                    ViewBag.ProductUnitId = new SelectList(_repository.ProductUnits.Where(p => p.ProductId == model.ProductId), "Id", "ProdUnit", model.ProductUnitId);

                    return View(model);
                }

                model.CreatedBy = User.Identity.Name;
                model.CreatedDate = DateTime.Now;

                _repository.Shades.Add(model);
                int result = await _repository.SaveChangesAsync();

                if (model.ShadesImages != null)
                    await UploadProductImages(model);

                return RedirectToAction("Index", null, new { area = "Admin" });
            }

            // var prods = _repository.Products.ToList();
            // foreach (var prod in prods)
            // {
            //     var brand = await _repository.Brands.FindAsync(prod.BrandId);
            //     prod.ProductName = brand.BrandName + " - " + prod.ProductName;
            // }

            // ViewBag.ProductId = new SelectList(prods, "Id", "ProductName", model.ProductId);
            ViewBag.ProductId = new SelectList(_repository.Products, "Id", "ProductName", model.ProductId);
            ViewBag.ProductUnitId = new SelectList(_repository.ProductUnits.Where(p => p.ProductId == model.ProductId), "Id", "ProdUnit", model.ProductUnitId);

            return View(model);
        }

        public async Task UploadProductImages(Shade model)
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
                    ProductUnitId = model.ProductUnitId,
                    ProductImageUrl = FileName,
                    ShadeId = model.Id
                };

                _repository.ProductImages.Add(productImage);
                await _repository.SaveChangesAsync();
            }
        }

        private List<string> UploadedFiles(Shade model)
        {
            List<string> list = new List<string>();

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

            if (!System.IO.Directory.Exists(uploadsFolder))
                System.IO.Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ShadesImages.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (System.IO.File.Exists(Path.Combine(uploadsFolder, "TempZip.zip")))
                System.IO.File.Delete(Path.Combine(uploadsFolder, "TempZip.zip"));

            if (model.ShadesImages != null)
            {
                string uploadszip = Path.Combine(_webHostEnvironment.WebRootPath, "productimages");

                filePath = Path.Combine(uploadszip, "TempZip.zip");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ShadesImages.CopyTo(fileStream);
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
            Shade model = await _repository.Shades.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            var prods = _repository.Products.ToList();
            foreach (var prod in prods)
            {
                var brand = await _repository.Brands.FindAsync(prod.BrandId);
                prod.ProductName = brand.BrandName + " - " + prod.ProductName;
            }

            //ViewBag.ProductId = new SelectList(prods, "Id", "ProductName", model.ProductId);

            ViewBag.ProductId = new SelectList(_repository.Products.OrderBy(p => p.ProductName), "Id", "ProductName", model.ProductId);
            var prd = await _repository.Products.FindAsync(model.ProductId);
            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName", prd.BrandId);

            var units = _repository.ProductUnits.Where(p => p.ProductId == model.ProductId);

            foreach (var unit in units)
            {
                unit.QuantityType = unit.Quantity + " " + unit.QuantityType;
            }

            ViewBag.ProductUnitId = new SelectList(units, "Id", "QuantityType", model.ProductUnitId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Shade product)
        {
            if (ModelState.IsValid)
            {
                product.UpdatedBy = User.Identity.Name;
                product.UpdatedDate = DateTime.Now;

                _repository.Entry(product).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                if (product.ShadesImages != null)
                    await UploadProductImages(product);

                return RedirectToAction("Index");
            }

            var prods = _repository.Products.ToList();
            foreach (var prod in prods)
            {
                var brand = await _repository.Brands.FindAsync(prod.BrandId);
                prod.ProductName = brand.BrandName + " - " + prod.ProductName;
            }

            //ViewBag.ProductId = new SelectList(prods, "Id", "ProductName", product.ProductId);

            ViewBag.ProductId = new SelectList(_repository.Products.OrderBy(p => p.ProductName), "Id", "ProductName", product.ProductId);
            var prd = await _repository.Products.FindAsync(product.ProductId);
            ViewBag.BrandId = new SelectList(_repository.Brands.OrderBy(p => p.BrandName), "Id", "BrandName", prd.BrandId);

            var units = _repository.ProductUnits.Where(p => p.ProductId == product.ProductId);

            foreach (var unit in units)
            {
                unit.QuantityType = unit.Quantity + " " + unit.QuantityType;
            }

            ViewBag.ProductUnitId = new SelectList(units, "Id", "QuantityType", product.ProductUnitId);
            return View(product);

        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportShade>>(await _repository.Shades.ToListAsync());

            foreach (var item in results)
            {
                var shade = await _repository.Shades.FindAsync(item.Id);

                item.ProductName = (await _repository.Products.FindAsync(shade.ProductId)).ProductName;
                item.ProductUnit = (await _repository.ProductUnits.FindAsync(shade.ProductUnitId)).GetPacking;
            }

            return _ex.Export<ExportShade>(results, "Shades", "Shades");
        }

        [Authorize(Roles = "superadmin")]
        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Shade shade = await _repository.Shades.FindAsync(id);

            var images = _repository.ProductImages.Where(p => p.ProductId == shade.ProductId && p.ProductUnitId == id).ToList();
            foreach (var img in images)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "productimages/", img.ProductImageUrl);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _repository.ProductImages.Remove(img);
                await _repository.SaveChangesAsync();
            }

            _repository.Shades.Remove(shade);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}