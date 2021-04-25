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
    public class StateController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public StateController(ApplicationDbContext repository,
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
            var models = _mapper.Map<IEnumerable<StateView>>(await _repository.States.ToListAsync());

            foreach (var model in models)
            {
                model.CoutryName = (await _repository.Countries.FirstOrDefaultAsync(p => p.Id == model.CountryId)).CountryName;
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(State model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.States.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", model.CountryId);
                return View(model);

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("IX_Countries_CountryCode"))
                    ModelState.AddModelError(string.Empty, "Country code already exists, please try another one.");
                else if (ex.InnerException.Message.Contains("IX_Countries_CountryName"))
                    ModelState.AddModelError(string.Empty, "Country name already exists, please try another one.");

                ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", model.CountryId);
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
            State model = await _repository.States.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", model.CountryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(State state)
        {
            if (ModelState.IsValid)
            {
                state.UpdatedBy = User.Identity.Name;
                state.UpdatedDate = DateTime.Now;

                _repository.Entry(state).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", state.CountryId);
            return View(state);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var results = _mapper.Map<List<ExportState>>(await _repository.States.ToListAsync());

            foreach (var item in results)
            {
                var state = await _repository.States.FindAsync(item.Id);
                item.CountryName = (await _repository.Countries.FindAsync(state.CountryId)).CountryName;
            }

            return _ex.Export<ExportState>(results, "States", "States");
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmedAssort(int id)
        {
            State state = await _repository.States.FindAsync(id);

            _repository.States.Remove(state);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}