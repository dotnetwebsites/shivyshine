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
using Shivyshine.Models;

namespace Shivyshine.Areas.Admin.Controllers
{
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    [Area("Admin")]
    public class ShippingMasterController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ShippingMasterController(ApplicationDbContext repository,
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
            var models = await _repository.ShippingMasters.ToListAsync();
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingMaster model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    _repository.ShippingMasters.Add(model);
                    int result = await _repository.SaveChangesAsync();

                    return RedirectToAction("Index", null, new { area = "Admin" });
                }

                ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", model.Pincode);
                return View(model);

            }
            catch (Exception ex)
            {
                var s = ex.InnerException.Message;

                ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", model.Pincode);
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
            ShippingMaster model = await _repository.ShippingMasters.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", model.Pincode);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ShippingMaster shippingMaster)
        {
            if (ModelState.IsValid)
            {
                shippingMaster.UpdatedBy = User.Identity.Name;
                shippingMaster.UpdatedDate = DateTime.Now;

                _repository.Entry(shippingMaster).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Pincode = new SelectList(_repository.Pincodes, "Pincodes", "Pincodes", shippingMaster.Pincode);
            return View(shippingMaster);
        }


        [Authorize(Roles = "admin")]
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmedAssort(int id)
        {
            ShippingMaster shippingMaster = await _repository.ShippingMasters.FindAsync(id);

            _repository.ShippingMasters.Remove(shippingMaster);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index", null, new { area = "Admin" });
        }

    }
}