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
using Shivyshine.Models;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class SerialNoController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public SerialNoController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // var models = _mapper.Map<IEnumerable<CityView>>(await _repository.Cities.ToListAsync());

            // foreach (var model in models)
            // {
            //     model.StateName = (await _repository.States.FirstOrDefaultAsync(p => p.Id == model.StateId)).StateName;
            // }
            var models = await _repository.SerialNoMasters.ToListAsync();
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SerialNoMaster model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serialNoMasters = await _repository.SerialNoMasters.Where(p => p.Type == model.Type).ToListAsync();

                    foreach (var sm in serialNoMasters)
                    {
                        sm.IsActive = false;
                        await _repository.SaveChangesAsync();
                    }

                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.SerialNoMasters.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                //ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", model.Pincode);
                return View(model);

            }
            catch (Exception ex)
            {
                var s = ex.InnerException.Message;

                //ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", model.Pincode);
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
            SerialNoMaster model = await _repository.SerialNoMasters.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SerialNoMaster serialNoMaster)
        {
            if (ModelState.IsValid)
            {
                serialNoMaster.UpdatedBy = User.Identity.Name;
                serialNoMaster.UpdatedDate = DateTime.Now;

                _repository.Entry(serialNoMaster).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(serialNoMaster);
        }

        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmedAssort(int id)
        {
            SerialNoMaster serialNoMaster = await _repository.SerialNoMasters.FindAsync(id);

            _repository.SerialNoMasters.Remove(serialNoMaster);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}