using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IExcel _ex;
        private readonly IMapper _mapper;

        public CountryController(ApplicationDbContext repository,
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
            var countries = await _repository.Countries.ToListAsync();
            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.Countries.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Countries_CountryCode"))
                    ModelState.AddModelError(string.Empty, "Country code already exists, please try another one.");
                else if (ex.InnerException.Message.Contains("IX_Countries_CountryName"))
                    ModelState.AddModelError(string.Empty, "Country name already exists, please try another one.");

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
            Country model = await _repository.Countries.FindAsync(id);
            model.ReturnUrl = returnUrl;
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Admin/Country");
            try
            {
                if (ModelState.IsValid)
                {
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;

                    _repository.Entry(model).State = EntityState.Modified;
                    await _repository.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }

                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Countries_CountryCode"))
                    ModelState.AddModelError(string.Empty, "Country code already exists, please try another one.");
                else if (ex.InnerException.Message.Contains("IX_Countries_CountryName"))
                    ModelState.AddModelError(string.Empty, "Country name already exists, please try another one.");

                return View(model);

            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportCountry>>(await _repository.Countries.ToListAsync());
            return _ex.Export<ExportCountry>(results, "Country", "Country");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Country country = await _repository.Countries.FindAsync(id);

            _repository.Countries.Remove(country);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}