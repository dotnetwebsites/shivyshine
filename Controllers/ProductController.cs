using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper,
                                UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            HomeViewModel model = new HomeViewModel();

            // model.Categories = await _repository.Categories.Take(8).ToListAsync();
            // model.SubCategories = await _repository.SubCategories.Take(8).ToListAsync();
            // model.SuperCategories = await _repository.SuperCategories.Take(8).ToListAsync();

            model.Brands = await _repository.Brands.ToListAsync();

            model.Products = await _repository.Products.Where(p => p.SuperCategoryId == id).ToListAsync();

            foreach (var item in model.Products)
            {
                item.IsPreviewShade = _repository.Shades.Any(p => p.ProductId == item.Id);
                item.IsPreviewSize = _repository.ProductUnits.Count(p => p.ProductId == item.Id) > 1;
            }

            model.ProductImages = await _repository.ProductImages.ToListAsync();

            return View(model);

            //var products = await _repository.Products.Where(p => p.SuperCategoryId == id).ToListAsync();
            //return View(products);
        }

        [HttpGet]
        public async Task<JsonResult> IsPincode(string id)
        {
            if (!_repository.Pincodes.Any(p => p.Pincodes == id))
            {
                Pincode err = new Pincode();
                err.Pincodes = "Sorry : Delivery not available for this pincode.";

                return Json(err);
            }

            var pincode = await _repository.Pincodes.FirstOrDefaultAsync(p => p.Pincodes == id);

            var city = await _repository.Cities.FirstOrDefaultAsync(p => p.Id == pincode.CityId);
            var state = await _repository.States.FirstOrDefaultAsync(p => p.Id == city.StateId);
            var country = await _repository.Countries.FirstOrDefaultAsync(p => p.Id == state.CountryId);

            var shipmast = await _repository.ShippingMasters.FirstOrDefaultAsync(p => p.Pincode.ToString() == pincode.Pincodes);

            Pincode code = new Pincode();
            code.Id = pincode.Id;
            code.LogMessage = pincode.Pincodes;
            code.Pincodes = "Shipping To: " + string.Concat(city.CityName, ", ",
            country.CountryName, " - ", pincode.Pincodes, " Delivered by ", shipmast.DeliveredBy + " ");

            CookieOptions option = new CookieOptions();
            Response.Cookies.Append("pincode", JsonConvert.SerializeObject(code), option);
            Response.Cookies.Append("zipcode", pincode.Pincodes, option);

            return Json(code);
        }

        public async Task<IActionResult> Details(int? id, int? unitid, int? shadeid = null)
        {
            if (id == null || unitid == null)
            {
                return NoContent();
            }

            ProductShadesImagesModel model = new ProductShadesImagesModel();

            model.Product = await _repository.Products.FindAsync(id);

            model.ProductUnit = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.Id == unitid);
            model.UnitImages = await _repository.ProductImages.Where(p => p.ProductId == id && p.ProductUnitId == unitid).ToListAsync();

            model.Shade = await _repository.Shades.FirstOrDefaultAsync(p => p.Id == shadeid);
            model.ShadeImages = await _repository.ProductImages.Where(p => p.ProductId == id && p.ShadeId == shadeid).ToListAsync();

            ViewBag.ShadeId = new SelectList(_repository.Shades, "Id", "ShadeName");
            return View(model);
        }

        [HttpGet]
        public JsonResult UpdateCart(int prodid, int unitid, int shadeid, int currqty)
        {
            double amount = 0; CookieOptions option = new CookieOptions();

            if (Request.Cookies["cart"] != null)
            {
                List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                foreach (var p in existingcarts)
                {
                    if (p.ProductId == prodid && p.ProductUnitId == unitid && p.ShadeId == shadeid)
                    {
                        var unit = _repository.ProductUnits.FirstOrDefault(p => p.Id == unitid);

                        amount = unit.Price * currqty;
                        p.Quantity = currqty;
                    }
                }

                if (!existingcarts.Any(p => p.ProductId == prodid && p.ProductUnitId == unitid && p.ShadeId == shadeid))
                {
                    CartViewModel cv = new CartViewModel();
                    cv.Id = 1;
                    cv.ProductId = prodid;
                    cv.ProductUnitId = unitid;
                    cv.ShadeId = shadeid;
                    cv.Quantity = 1;

                    existingcarts.Add(cv);
                }

                Response.Cookies.Append("cart", JsonConvert.SerializeObject(existingcarts.ToList()), option);
                return Json(existingcarts);
            }
            else
            {
                List<CartViewModel> carts = new List<CartViewModel>();
                CartViewModel cv = new CartViewModel();
                cv.Id = 1;
                cv.ProductId = prodid;
                cv.ProductUnitId = unitid;
                cv.ShadeId = shadeid;
                cv.Quantity = currqty;

                carts.Add(cv);

                Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                return Json(carts);
            }

            //return Json(amount.ToString("#.00"));            
        }

        // [HttpGet]
        // public async Task<IActionResult> ReviewRatings(int id, int unitid, int shadeid,
        // string name, string email, string comment, int rate)
        // {
        //     if (!(_repository.Products.Any(p => p.Id == id)))
        //         ModelState.AddModelError(string.Empty, "Product is invalid");

        //     if (!(_repository.ProductUnits.Any(p => p.Id == unitid)))
        //         ModelState.AddModelError(string.Empty, "Product unit is invalid");

        //     if (!(_repository.Shades.Any(p => p.Id == shadeid)))
        //         ModelState.AddModelError(string.Empty, "shades is invalid");

        //     if (name == null || name == "")
        //         ModelState.AddModelError(string.Empty, "Please enter name");

        //     if (email == null || email == "")
        //         ModelState.AddModelError(string.Empty, "Please enter email");

        //     if (comment == null || comment == "")
        //         ModelState.AddModelError(string.Empty, "Please enter review text");

        //     if (!(rate >= 1 && rate <= 5))
        //         ModelState.AddModelError(string.Empty, "Please select rating");

        //     var cnt = ModelState.Values.Where(v => v.Errors.Count != 0).Count();

        //     if (cnt > 0)
        //     {
        //         ProductShadesImagesModel model = new ProductShadesImagesModel();

        //         model.Product = await _repository.Products.FindAsync(id);

        //         model.ProductUnit = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.Id == unitid);
        //         model.UnitImages = await _repository.ProductImages.Where(p => p.ProductId == id && p.ProductUnitId == unitid).ToListAsync();

        //         model.Shade = await _repository.Shades.FirstOrDefaultAsync(p => p.Id == shadeid);
        //         model.ShadeImages = await _repository.ProductImages.Where(p => p.ProductId == id && p.ShadeId == shadeid).ToListAsync();

        //         ViewBag.ShadeId = new SelectList(_repository.Shades, "Id", "ShadeName");

        //         return View("Details", model);
        //     }

        //     ProductReview productReview = new ProductReview();
        //     productReview.ProductId = id;
        //     productReview.UnitId = unitid;
        //     productReview.ShadeId = shadeid;
        //     productReview.Name = name;
        //     productReview.Email = email;
        //     productReview.ReviewText = comment;
        //     productReview.Rating = rate;

        //     _repository.ProductReviews.Add(productReview);
        //     await _repository.SaveChangesAsync();

        //     return RedirectToAction("Details", "Product", new { Area = "", id = id });
        // }

        // [HttpGet]
        // public async Task<IActionResult> ReviewRatings(ProductReview pr)
        // {
        //     if (!(_repository.Products.Any(p => p.Id == pr.ProductId)))
        //         ModelState.AddModelError(string.Empty, "Product is invalid");

        //     if (!(_repository.ProductUnits.Any(p => p.Id == pr.UnitId)))
        //         ModelState.AddModelError(string.Empty, "Product unit is invalid");

        //     if (!(_repository.Shades.Any(p => p.Id == pr.ShadeId)))
        //         ModelState.AddModelError(string.Empty, "shades is invalid");

        //     if (pr.Name == null || pr.Name == "")
        //         ModelState.AddModelError(string.Empty, "Please enter name");

        //     if (pr.Email == null || pr.Email == "")
        //         ModelState.AddModelError(string.Empty, "Please enter email");

        //     if (pr.ReviewText == null || pr.ReviewText == "")
        //         ModelState.AddModelError(string.Empty, "Please enter review text");

        //     if (!(pr.Rating >= 1 && pr.Rating <= 5))
        //         ModelState.AddModelError(string.Empty, "Please select rating");

        //     //var cnt = ModelState.Values.Where(v => v.Errors.Count != 0).Count();

        //     if (ModelState.IsValid)
        //     {
        //         // ProductReview productReview = new ProductReview();
        //         // productReview.ProductId = pr.ProductId;
        //         // productReview.UnitId = pr.UnitId;
        //         // productReview.ShadeId = pr.ShadeId;
        //         // productReview.Name = pr.Name;
        //         // productReview.Email = pr.Email;
        //         // productReview.ReviewText = pr.ReviewText;
        //         // productReview.Rating = pr.Rating;

        //         pr.CreatedBy = User.Identity.Name;
        //         pr.CreatedDate = DateTime.Now;

        //         _repository.ProductReviews.Add(pr);
        //         await _repository.SaveChangesAsync();

        //         return RedirectToAction("Details", "Product", new { Area = "", id = pr.ProductId });
        //     }

        //     ProductShadesImagesModel model = new ProductShadesImagesModel();

        //     model.Product = await _repository.Products.FindAsync(pr.ProductId);

        //     model.ProductUnit = await _repository.ProductUnits.FirstOrDefaultAsync(p => p.Id == pr.UnitId);
        //     model.UnitImages = await _repository.ProductImages.Where(p => p.ProductId == pr.ProductId && p.ProductUnitId == pr.UnitId).ToListAsync();

        //     model.Shade = await _repository.Shades.FirstOrDefaultAsync(p => p.Id == pr.ShadeId);
        //     model.ShadeImages = await _repository.ProductImages.Where(p => p.ProductId == pr.ShadeId && p.ShadeId == pr.ShadeId).ToListAsync();

        //     ViewBag.ShadeId = new SelectList(_repository.Shades, "Id", "ShadeName");

        //     return View("Details", model);
        // }

        [HttpGet]
        public JsonResult GetQuantity(int prodid, int unitid, int shadeid)
        {
            int qty = 1;
            if (Request.Cookies["cart"] != null)
            {
                List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                if (shadeid > 0)
                    qty = existingcarts.FirstOrDefault(p => p.ProductId == prodid && p.ProductUnitId == unitid && p.ShadeId == shadeid).Quantity;
                else if (existingcarts.Any(p => p.ProductId == prodid && p.ProductUnitId == unitid))
                    qty = existingcarts.FirstOrDefault(p => p.ProductId == prodid && p.ProductUnitId == unitid).Quantity;

                return Json(qty);
            }

            return Json(qty);
        }

        [HttpPost]
        public async Task<IActionResult> SaveReview(ProductReview model)
        {
            if (model.Name == null)
                return Json("User is not valid");

            if (model.Email == null)
                return Json("Location not entered, please enter destination.");

            //ProductReview review = new ProductReview();
            //review.ProductId = model.ProductId;

            //review.Name = model.Name;
            //review.Email = model.Email;
            //review.ReviewText = model.ReviewText;
            //review.Rating = model.Rating;

            var user = await _userManager.GetUserAsync(User);
            
            model.CreatedBy = user.Id;
            model.CreatedDate = DateTime.Now;

            _repository.ProductReviews.Add(model);
            _repository.SaveChanges();

            return Json(model);

            //if (model.ShadeId > 0)
            //    return RedirectToAction("Details", "Product", new { id = model.ProductId, unitid = model.UnitId, shadeid = model.ShadeId });
            //else
            //    return RedirectToAction("Details", "Product", new { id = model.ProductId, unitid = model.UnitId });
        }

    }
}