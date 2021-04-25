using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IExcel _ex;
        private readonly IMapper _mapper;

        public BrandController(ApplicationDbContext repository, IExcel ex, IMapper mapper)
        {
            _repository = repository;
            _ex = ex;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _repository.Brands.ToListAsync();
            return View(brands);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Error();
            }
            Brand brand = await _repository.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.Name;
                model.CreatedDate = DateTime.Now;

                _repository.Brands.Add(model);
                int result = await _repository.SaveChangesAsync();

                return RedirectToAction("Index", null, new { area = "Admin" });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            Brand model = await _repository.Brands.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Brand model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/Brand");
            try
            {
                if (ModelState.IsValid)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;

                    _repository.Entry(model).State = EntityState.Modified;
                    await _repository.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                    //return RedirectToAction("Index");
                }

                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Brands_BrandName"))
                    ModelState.AddModelError(string.Empty, "Brand name already exist, please try another one.");
                else
                    ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);

            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            //var results = await _repository.Brands.ToListAsync();
            var results = _mapper.Map<List<ExportBrand>>(await _repository.Brands.ToListAsync());
            return _ex.Export<ExportBrand>(results, "Brand", "BrandSheet");
        }

        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Brand brand = await _repository.Brands.FindAsync(id);

            _repository.Brands.Remove(brand);
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