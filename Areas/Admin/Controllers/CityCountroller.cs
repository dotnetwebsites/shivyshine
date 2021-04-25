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
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;        

        public CityController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IExcel ex, IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _ex = ex;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var models = _mapper.Map<IEnumerable<CityView>>(await _repository.Cities.ToListAsync());

            foreach (var model in models)
            {
                model.StateName = (await _repository.States.FirstOrDefaultAsync(p => p.Id == model.StateId)).StateName;
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.Cities.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", model.StateId);
                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Countries_CountryCode"))
                    ModelState.AddModelError(string.Empty, "Country code already exists, please try another one.");
                else if (ex.InnerException.Message.Contains("IX_Countries_CountryName"))
                    ModelState.AddModelError(string.Empty, "Country name already exists, please try another one.");

                ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", model.StateId);
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
            City model = await _repository.Cities.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", model.StateId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(City city)
        {
            if (ModelState.IsValid)
            {
                city.UpdatedBy = User.Identity.Name;
                city.UpdatedDate = DateTime.Now;

                _repository.Entry(city).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", city.StateId);
            return View(city);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportCity>>(await _repository.Cities.ToListAsync());

            foreach (var item in results)
            {
                var city = await _repository.Cities.FindAsync(item.Id);
                item.StateName = (await _repository.States.FindAsync(city.StateId)).StateName;
            }

            return _ex.Export<ExportCity>(results, "City", "City");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmedAssort(int id)
        {
            City city = await _repository.Cities.FindAsync(id);

            if (await _repository.Pincodes.AnyAsync(p => p.CityId == id))
            {
                ModelState.AddModelError(string.Empty, "Pincodes are linked with this city, first delete all picodes which linked and try again.");

                var cities = await _repository.Cities.ToListAsync();
                return View("Index", cities);
            }

            _repository.Cities.Remove(city);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}