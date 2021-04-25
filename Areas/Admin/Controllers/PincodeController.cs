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
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class PincodeController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;
        
        public PincodeController(ApplicationDbContext repository,
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
            var models = _mapper.Map<IEnumerable<PincodeView>>(await _repository.Pincodes.ToListAsync());

            foreach (var model in models)
            {
                model.CityName = (await _repository.Cities.FirstOrDefaultAsync(p => p.Id == model.CityId)).CityName;
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pincode model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.Pincodes.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", model.CityId);
                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Countries_CountryCode"))
                    ModelState.AddModelError(string.Empty, "Country code already exists, please try another one.");
                else if (ex.InnerException.Message.Contains("IX_Countries_CountryName"))
                    ModelState.AddModelError(string.Empty, "Country name already exists, please try another one.");

                ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", model.CityId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportPincode>>(await _repository.Pincodes.ToListAsync());

            foreach (var item in results)
            {
                var pincode = await _repository.Pincodes.FindAsync(item.Id);
                item.CityName = (await _repository.Cities.FindAsync(pincode.CityId)).CityName;
            }

            return _ex.Export<ExportPincode>(results, "Pincode", "Pincode");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            Pincode pincode = await _repository.Pincodes.FindAsync(id);

            _repository.Pincodes.Remove(pincode);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}