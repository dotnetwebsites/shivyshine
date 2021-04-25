using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class PurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PurchaseOrderController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var models = _mapper.Map<IEnumerable<PurchaseOrderView>>(await _repository.PurchaseOrders.ToListAsync());

            foreach (var model in models)
            {
                model.VendorName = (await _repository.VendorMasters.FindAsync(model.VendorId)).VendorName;
            }

            return View(models);
        }

        // [HttpGet]
        // public async Task<JsonResult> LoadBrands(int id)
        // {
        //     var brands = await _repository.Brands
        //                         .FromSqlRaw(@"select distinct b.* from customerorders c
        //                         left join customerorderassorts a on c.id=a.id
        //                         left join Products p on p.id = a.ProductId
        //                         left join Brands b on b.id=p.brandid").ToListAsync();

        //     return Json(brands);
        // }

        [HttpGet]
        public async Task<JsonResult> LoadProducts(string id)
        {
            TempData["POView"] = await _repository.POViewModels
                        .FromSqlRaw(@"select a.ProductId,a.UnitId,a.ShadeId,pu.MRP,
                        SUM(a.Quantity) Quantity,p.ProductName + ' ' + CONVERT(varchar,pu.Quantity) + ' ' + 
                        pu.QuantityType + ' ' + ISNULL(s.ShadeName,'') ProductName
                        from customerorderassorts a 
                        left join Products p on p.id = a.ProductId
                        left join ProductUnits pu on pu.id = a.UnitId
                        left join Shades s on s.id = a.ShadeId
                        left join Brands b on b.id=p.brandid
                        where b.id = 2
                        group by a.ProductId,a.UnitId,a.ShadeId,pu.MRP,
                        p.ProductName + ' ' + CONVERT(varchar,pu.Quantity) + ' ' + 
                        pu.QuantityType + ' ' + ISNULL(s.ShadeName,'')", id).ToListAsync();
            return Json("ok");
        }

        [HttpGet]
        public async Task<ActionResult> Create(int vend, string brands)
        {
            if (vend <= 0)
            {
                ViewBag.VendorId = new SelectList(_repository.VendorMasters, "Id", "VendorName");
                return View();
            }

            var vendor = await _repository.VendorMasters.FindAsync(vend);

            if (vendor == null)
            {
                ViewBag.VendorId = new SelectList(_repository.VendorMasters, "Id", "VendorName");
                ModelState.AddModelError(string.Empty, "Invalid Vendor, please select a valid vendor and try again.");
                return View();
            }

            ViewBag.VendorId = new SelectList(_repository.VendorMasters, "Id", "VendorName", vend);

            // ViewBag.BrandId = new SelectList(await _repository.Brands
            //     .FromSqlRaw(@"select distinct b.* from customerorders c
            //                     left join customerorderassorts a on c.id=a.id
            //                     left join Products p on p.id = a.ProductId
            //                     left join Brands b on b.id=p.brandid").ToListAsync(), "Id", "BrandName");

            TempData["Brands"] = await _repository.Brands
                                .FromSqlRaw(@"select distinct b.* from customerorders c
                                left join customerorderassorts a on c.id=a.id
                                left join Products p on p.id = a.ProductId
                                left join Brands b on b.id=p.brandid
								left join vendorbrands v on v.BrandId=b.Id
								where v.VendorId=" + vend).ToListAsync();

            if (brands != null)
            {
                string[] sbrands = brands.Split(',');

                StringBuilder sb = new StringBuilder();

                foreach (string s in sbrands)
                {
                    sb.Append("insert into @tbl select " + s + ";");
                }

                TempData["POView"] = await _repository.POViewModels
                    .FromSqlRaw(@"declare @tbl table(id int)
                    " + sb.ToString() + @"
                    select a.ProductId,a.UnitId,a.ShadeId,pu.MRP,
                    SUM(a.Quantity) Quantity,p.ProductName + ' ' + CONVERT(varchar,pu.Quantity) + ' ' + 
                    pu.QuantityType + ' ' + ISNULL(s.ShadeName,'') ProductName
                    from customerorderassorts a 
                    left join Products p on p.id = a.ProductId
                    left join ProductUnits pu on pu.id = a.UnitId
                    left join Shades s on s.id = a.ShadeId
                    left join Brands b on b.id=p.brandid
                    where b.id in (select id from @tbl)
                    group by a.ProductId,a.UnitId,a.ShadeId,pu.MRP,
                    p.ProductName + ' ' + CONVERT(varchar,pu.Quantity) + ' ' + 
                    pu.QuantityType + ' ' + ISNULL(s.ShadeName,'')").ToListAsync();
            }


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(POSaveModel po)
        {
            PurchaseOrder pord = new PurchaseOrder();
            pord.PODate = po.PODate;
            pord.VendorId = po.VendorId;

            pord.CreatedBy = User.Identity.Name;
            pord.CreatedDate = DateTime.Now;
            await _repository.PurchaseOrders.AddAsync(pord);
            _repository.SaveChanges();

            string[] assorts = po.Assorts.Split('~');

            foreach (var a in assorts)
            {
                int prodid = Convert.ToInt32(a.Split(',')[0].Split(':')[1].ToString());
                int unitid = Convert.ToInt32(a.Split(',')[1].Split(':')[1].ToString());
                int shdid = Convert.ToInt32(a.Split(',')[2].Split(':')[1].ToString());
                int qty = Convert.ToInt32(a.Split(',')[3].Split(':')[1].ToString());

                POAssort poa = new POAssort();
                poa.POId = pord.Id;
                poa.ProductId = prodid;
                poa.UnitId = unitid;
                poa.ShadeId = shdid;
                poa.Quantity = qty;
                poa.CreatedBy = User.Identity.Name;
                poa.CreatedDate = DateTime.Now;

                await _repository.POAssorts.AddAsync(poa);
                _repository.SaveChanges();
            }


            return RedirectToAction("Create");
        }


        // [Authorize(Roles = "admin")]
        // [HttpGet, ActionName("Delete")]
        // public async Task<ActionResult> DeleteConfirmedAssort(int id)
        // {
        //     PurchaseOrder purchaseOrder = await _repository.PurchaseOrders.FindAsync(id);

        //     var assorts = _repository.POAssorts.Where(p => p.POId == id).ToList();
            
        //     foreach (var assort in assorts)
        //     {
                
        //     }
            
        //     _repository.Products.Remove(product);
        //     await _repository.SaveChangesAsync();
        //     return RedirectToAction("Index", null, new { area = "Admin" });
        // }

    }
}