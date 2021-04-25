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

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class VendorBrandController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public VendorBrandController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var models = _mapper.Map<IEnumerable<VendorBrandView>>(await _repository.vendorBrands.ToListAsync());

            foreach (var model in models)
            {
                var vend = await _repository.VendorMasters.FindAsync(model.VendorId);
                var brnd = await _repository.Brands.FindAsync(model.BrandId);

                model.VendorName = vend.VendorName;
                model.BrandName = brnd.BrandName;
            }


            //var models = await _repository.vendorBrands.ToListAsync();
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName");
            ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorBrand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.vendorBrands.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName", model.VendorId);
                ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName", model.BrandId);
                return View(model);

            }
            catch (Exception ex)
            {
                ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName", model.VendorId);
                ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName", model.BrandId);
                var s = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            VendorBrand model = await _repository.vendorBrands.FindAsync(id);

            ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName", model.VendorId);
            ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName", model.BrandId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VendorBrand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;

                    _repository.Entry(model).State = EntityState.Modified;
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName", model.VendorId);
                ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName", model.BrandId);
                return View(model);

            }
            catch (Exception ex)
            {
                ViewBag.VendorId = new SelectList(_repository.VendorMasters.ToList(), "Id", "VendorName", model.VendorId);
                ViewBag.BrandId = new SelectList(_repository.Brands.ToList(), "Id", "BrandName", model.BrandId);
                var s = ex.Message;
                return View(model);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            VendorBrand vendorBrand = await _repository.vendorBrands.FindAsync(id);

            _repository.vendorBrands.Remove(vendorBrand);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}