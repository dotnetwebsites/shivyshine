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
    public class CustomerOrdersController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public CustomerOrdersController(ApplicationDbContext repository,
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
            List<MyOrderView> mv = new List<MyOrderView>();

            var customerOrders = await _repository.CustomerOrders.ToListAsync();

            foreach (var co in customerOrders)
            {
                var assorts = await _repository.CustomerOrderAssorts.Where(p => p.CustOrderId == co.Id).ToListAsync();
                double netamount = 0; int NoOfItems = 0; double charges = 0;

                foreach (var a in assorts)
                {
                    var product = await _repository.Products.FirstOrDefaultAsync(p => p.Id == a.ProductId);
                    var unit = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.Id == a.UnitId);
                    var shd = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.Id == a.ShadeId);

                    NoOfItems = NoOfItems + a.Quantity;
                    netamount = netamount + (unit.Price * a.Quantity);
                }

                charges = _repository.ShippingMasters.FirstOrDefault(p => p.Pincode == co.Pincode && p.MinAmount > netamount)?.ShippingCharge ?? 0;

                MyOrderView m = new MyOrderView();
                m.CustId = co.Id;
                m.OrderNo = co.OrderNo;
                m.OrderDate = co.OrderDate;
                m.OrderStatus = "";
                m.NetPayable = netamount + charges;
                m.NoOfItems = NoOfItems;
                m.DeliveryStatus = "";
                m.PayMode = co.PaymentMode;
                m.IsOrderCancel = co.IsOrderCancel;

                if (co.PaymentMode == "BANK")
                    m.PayId = co.PaymentId;

                mv.Add(m);
            }

            return View(mv);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> OrderDetails(int? id)
        {
            var cust = await _repository.CustomerOrders.FirstOrDefaultAsync(p => p.Id == id && p.Username == User.Identity.Name);
            var assorts = await _repository.CustomerOrderAssorts.Where(p => p.CustOrderId == cust.Id).ToListAsync();
            List<MyOrderAssortsView> assortlists = new List<MyOrderAssortsView>();

            foreach (var a in assorts)
            {
                var unit = _repository.ProductUnits.FirstOrDefault(p => p.Id == a.UnitId);

                MyOrderAssortsView orderAssortsView = new MyOrderAssortsView();

                if (a.ShadeId > 0 && a.ShadeId != null)
                {
                    var shadeimg = _repository.ProductImages.FirstOrDefault(p => p.ProductId == a.ProductId
                    && p.ProductUnitId == a.UnitId
                    && p.ShadeId == a.ShadeId);

                    orderAssortsView.ProdImgUrl = shadeimg.ProductImageUrl;
                }
                else
                {
                    var pimg = _repository.ProductImages.FirstOrDefault(p => p.ProductId == a.ProductId
                    && p.ProductUnitId == a.UnitId);

                    orderAssortsView.ProdImgUrl = pimg.ProductImageUrl;
                }

                orderAssortsView.CustId = a.CustOrderId;

                orderAssortsView.ProductName = _repository.Products.FirstOrDefault(p => p.Id == a.ProductId).ProductName;
                orderAssortsView.Quantity = a.Quantity;
                orderAssortsView.Price = unit.Price;

                assortlists.Add(orderAssortsView);
            }

            return View(assortlists);
        }
    }
}