using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;

namespace Shivyshine.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly INumberSeries _numberSeries;
        public IConfiguration _configuration;


        public HomeController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper,
                                SignInManager<ApplicationUser> signInManager,
                                UserManager<ApplicationUser> userManager,
                                INumberSeries numberSeries,
                                IConfiguration configuration)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _numberSeries = numberSeries;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]        
        public async Task<IActionResult> Index()
        {
            if (Request.Cookies["payid"] != null)
                Response.Cookies.Delete("payid");
            // if (Request.Cookies["payid"] == null)
            // {
            //     CookieOptions op = new CookieOptions();
            //     Response.Cookies.Append("payid", "21", op);
            // }

            HomeViewModel model = new HomeViewModel();

            model.Categories = await _repository.Categories.ToListAsync();
            model.SubCategories = await _repository.SubCategories.ToListAsync();
            model.SuperCategories = await _repository.SuperCategories.Where(p => p.IsDisplay).ToListAsync();

            model.Products = await _repository.Products.Where(p => p.IsDisplay).ToListAsync();

            foreach (var item in model.Products)
            {
                item.IsPreviewShade = await _repository.Shades.AnyAsync(p => p.ProductId == item.Id);
                item.IsPreviewSize = _repository.ProductUnits.Count(p => p.ProductId == item.Id) > 1;
            }

            model.ProductImages = await _repository.ProductImages.ToListAsync();

            model.MainBanners = await _repository.MainBanners.ToListAsync();
            model.SubBanners = await _repository.SubBanners.ToListAsync();

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AddToCart(int id, int unitid, int shadeid)
        {
            if (unitid > 0)
            {
                //var unitprod = _repository.ProductUnits.FirstOrDefault(p => p.Id == unitid && p.ProductId == id);
                CookieOptions option = new CookieOptions();
                List<CartViewModel> carts = new List<CartViewModel>();

                if (Request.Cookies["cart"] != null)
                {
                    List<CartViewModel> existingcarts = JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                    foreach (var cart in existingcarts)
                    {
                        if (cart.ProductId == id && cart.ProductUnitId == unitid && cart.ShadeId == shadeid)
                        {
                            cart.Id = cart.Id + 1;
                            cart.ProductId = id;
                            cart.ProductUnitId = unitid;
                            cart.ShadeId = shadeid;
                            cart.Quantity = cart.Quantity + 1;
                            cart.Pincode = cart.Pincode;

                            carts.Add(cart);
                        }
                        else
                            carts.Add(cart);
                    }

                    if (!existingcarts.Any(p => p.ProductId == id && p.ProductUnitId == unitid && p.ShadeId == shadeid))
                    {
                        CartViewModel cv = new CartViewModel();
                        cv.Id = 1;
                        cv.ProductId = id;
                        cv.ProductUnitId = unitid;
                        cv.ShadeId = shadeid;
                        cv.Quantity = 1;

                        carts.Add(cv);
                    }


                    Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                    return RedirectToAction("Index");
                }
                else
                {
                    CartViewModel cv = new CartViewModel();
                    cv.Id = 1;
                    cv.ProductId = id;
                    cv.ProductUnitId = unitid;
                    cv.ShadeId = shadeid;
                    cv.Quantity = 1;

                    carts.Add(cv);

                    Response.Cookies.Append("cart", JsonConvert.SerializeObject(carts.ToList()), option);
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult UpdateCart(int prodid, int unitid, int shadeid, int currqty)
        {
            double amount = 0;
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

                CookieOptions option = new CookieOptions();
                Response.Cookies.Append("cart", JsonConvert.SerializeObject(existingcarts.ToList()), option);
                //return Json(existingcarts.ToList());
            }

            return Json(amount.ToString("#.00"));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cart()
        {
            if (Request.Cookies["cart"] == null)
                return RedirectToAction("Index");
            else
                return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Checkout()
        {
            if (Request.Cookies["cart"] == null)
                return RedirectToAction("Index");
            else
            {
                ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName");
                return View();
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DeleteToCart(int pid, int unitid, int shadeid)
        {
            CookieOptions option = new CookieOptions();
            if (Request.Cookies["cart"] != null)
            {
                List<CartViewModel> existingcarts =
                JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"]?.ToString());

                CartViewModel model = existingcarts.FirstOrDefault(p => p.ProductId == pid &&
                p.ProductUnitId == unitid && p.ShadeId == shadeid);
                existingcarts.Remove(model);

                Response.Cookies.Append("cart", JsonConvert.SerializeObject(existingcarts.ToList()), option);
                if (existingcarts.Count() > 0)
                    return RedirectToAction("Cart");
                else
                    return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AddNewAddress()
        {
            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName");
            return View("Checkout");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddNewAddress(Address model)
        {
            if (ModelState.IsValid)
            {
                if (!_repository.Pincodes.Any(p => Convert.ToInt32(p.Pincodes) == model.Pincode))
                {
                    ModelState.AddModelError(string.Empty, "Sorry : Delivery not available for this pincode.");
                    return View("Checkout", model);
                }

                var pincode = _repository.Pincodes.FirstOrDefault(p => Convert.ToInt32(p.Pincodes) == model.Pincode);
                var city = _repository.Cities.FirstOrDefault(p => p.Id == pincode.CityId);
                var state = _repository.States.FirstOrDefault(p => p.Id == city.StateId);
                var country = _repository.Countries.FirstOrDefault(p => p.Id == state.CountryId);

                if (pincode == null || city == null || state == null || country == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Pincode");
                    return View("Checkout", model);
                }

                model.CountryId = country.Id;
                model.StateId = state.Id;
                model.CityId = city.Id;
                model.Pincode = Convert.ToInt32(pincode.Pincodes);


                model.Username = User.Identity.Name;
                model.CreatedBy = User.Identity.Name;
                model.CreatedDate = DateTime.Now;

                _repository.Addresses.Add(model);
                int result = await _repository.SaveChangesAsync();
                return RedirectToAction("Checkout");
            }

            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", model.CountryId);
            return View("Checkout", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(int id, int pmode)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!_signInManager.IsSignedIn(User))
                return Json("Not signin");

            if (Request.Cookies["cart"] == null)
                return Json("No items available in cart");

            if (!(await _repository.Addresses.AnyAsync(p => p.Id == id && p.Username == User.Identity.Name)))
                return Json("Invalid Address");

            if (!(pmode == 1 || pmode == 2))
                return Json("Mode of payment is not valid, please try again");


            bool pbank = false; Guid payId = new Guid();
            PaymentInitiateModel pm = new PaymentInitiateModel();
            List<CartViewModel> carts =
            JsonConvert.DeserializeObject<List<CartViewModel>>(Request.Cookies["cart"].ToString());

            var address = await _repository.Addresses.FirstOrDefaultAsync(p => p.Id == id && p.Username == User.Identity.Name);
            if (!_repository.Pincodes.Any(p => Convert.ToInt32(p.Pincodes) == address.Pincode))
            {
                return Json("Pincode is not valid");
            }

            var ship = await _repository.ShippingMasters.FirstOrDefaultAsync(p => p.Pincode == address.Pincode);

            CustomerOrder order = new CustomerOrder();

            order.Username = User.Identity.Name;
            order.OrderNo = _numberSeries.GenerateOrderNumber(User.Identity.Name);
            order.OrderDate = DateTime.Now;

            order.Pincode = address.Pincode;
            order.AddressId = address.Id;

            order.ShippingId = ship.Id;
            order.ShippingCharges = ship.ShippingCharge;

            order.CreatedBy = User.Identity.Name;
            order.CreatedDate = DateTime.Now;

            if (pmode == 1)
            {
                var ads = await _repository.Addresses.FindAsync(order.AddressId);
                var cntr = await _repository.Countries.FindAsync(ads.CountryId);
                var sts = await _repository.States.FindAsync(ads.StateId);
                var ct = await _repository.Cities.FindAsync(ads.CityId);

                double totalamount = 0, charges = 0;
                foreach (var assort in carts)
                {
                    var unit = await _repository.ProductUnits.FindAsync(assort.ProductUnitId);
                    var shade = await _repository.Shades.FindAsync(assort.ShadeId);

                    if (shade != null && shade.IsVisiblePrice)
                        totalamount = totalamount + shade.AmountCalc(shade.Price, assort.Quantity);
                    else
                        totalamount = totalamount + unit.AmountCalc(unit.Price, assort.Quantity);
                }

                charges = (await _repository.ShippingMasters.FirstOrDefaultAsync(p => p.Pincode == ads.Pincode && p.MinAmount > totalamount))?.ShippingCharge ?? 0;

                pm.Id = Guid.NewGuid();
                payId = pm.Id;
                pm.Name = user.FullName;
                pm.Email = user.Email;
                pm.ContactNumber = user.PhoneNumber;
                pm.Amount = totalamount + charges;

                pm.Address = ads.FullAddress + ", " + ct.CityName + ", " + sts.StateName + ", " + cntr.CountryName + ", " + ads.Pincode;

                pm.CreatedBy = User.Identity.Name;
                pm.CreatedDate = DateTime.Now;

                _repository.PaymentInitiateModels.Add(pm);
                await _repository.SaveChangesAsync();

                order.PaymentMode = "BANK";
                order.PaymentId = pm.Id;
                pbank = true;
            }
            else if (pmode == 2)
                order.PaymentMode = "COD";

            _repository.CustomerOrders.Add(order);
            int result = await _repository.SaveChangesAsync();

            foreach (var assort in carts)
            {
                CustomerOrderAssort a = new CustomerOrderAssort();
                a.CustOrderId = order.Id;
                a.ProductId = assort.ProductId;
                a.UnitId = assort.ProductUnitId;
                a.ShadeId = assort.ShadeId;
                a.Quantity = assort.Quantity;

                _repository.CustomerOrderAssorts.Add(a);
                await _repository.SaveChangesAsync();
            }

            //need to add here a log for order status
            OrderStatus status = new OrderStatus();
            status.CustId = order.Id;
            status.Username = User.Identity.Name;
            status.OrdStatus = ORDSTATUS.PLACED.ToString();
            status.Remark = "Order Placed";
            status.CreatedBy = User.Identity.Name;
            status.CreatedDate = DateTime.Now;
            _repository.OrderStatuses.Add(status);
            await _repository.SaveChangesAsync();

            Response.Cookies.Delete("cart");
            //var customerOrder = await _repository.CustomerOrders.ToListAsync();

            SendSms sms = new SendSms(order.OrderNo, order.OrderDate);

            var sURL = _configuration["SmsUrl"].ToString() +
                        // "user=" + _configuration["SMSUser"].ToString() +
                        // "&password=" + _configuration["SMSPass"].ToString() +
                        "APIKey=" + _configuration["SMSApi"].ToString() +
                        "&senderid=" + _configuration["SenderName"].ToString() +
                        "&channel=" + _configuration["SMSChnl"].ToString() +
                        "&DCS=" + _configuration["SMSDCS"].ToString() +
                        "&flashsms=" + _configuration["SMSFlash"].ToString() +
                        "&number=" + user.PhoneNumber +
                        "&text=" + sms.Message +
                        "&route=" + _configuration["SmsRoute"].ToString();

            if (pbank)
            {
                CookieOptions option = new CookieOptions();
                Response.Cookies.Append("payid", payId.ToString(), option);
                //Response.Cookies.Append("payid", order.Id.ToString(), option);
                return Json("bnk");
            }
            else
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {

                        string s = client.DownloadString(sURL);

                        var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(s);
                        int n = responseObject.Status;
                        // if (n == 3)
                        // {
                        //     Json("Message does not Send Successfully due to invalid credentials");
                        // }
                        // else
                        // {
                        //     Json("Message Send Successfully !");                        
                        // }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error in sending Message");
                    ex.ToString();
                }

                CookieOptions option = new CookieOptions();
                //Response.Cookies.Append("payid", order.Id.ToString(), option);
                Response.Cookies.Append("payid", payId.ToString(), option);
                return Json("csh");
            }
        }



        // [HttpPost]
        // public async Task<IActionResult> PlaceOrder([FromBody] List<CustomerOrderView> models)
        // {
        //     if (!_signInManager.IsSignedIn(User))
        //         return Json("Not signin");

        //     CustomerOrder order = new CustomerOrder();
        //     order.Username = User.Identity.Name;
        //     order.OrderNo = _numberSeries.GenerateOrderNumber(User.Identity.Name);
        //     order.OrderDate = DateTime.Now;

        //     order.Pincode = models[0].Pincode;
        //     order.AddressId = models[0].AddressId;

        //     order.CreatedBy = User.Identity.Name;
        //     order.CreatedDate = DateTime.Now;
        //     order.ShippingCharges = 
        //     _repository.ShippingMasters.FirstOrDefault(p=>p.Pincode == 
        //     Convert.ToUInt32(order.Pincode) && p.MinAmount > totalamount);

        //     _repository.CustomerOrders.Add(order);
        //     int result = await _repository.SaveChangesAsync();

        //     foreach (var assort in models)
        //     {
        //         CustomerOrderAssort a = new CustomerOrderAssort();
        //         a.CustOrderId = order.Id;
        //         a.ProductId = assort.ProductId;
        //         a.UnitId = assort.ProductUnitId;
        //         a.ShadeId = assort.ShadeId;
        //         a.Quantity = assort.Quantity;

        //         _repository.CustomerOrderAssorts.Add(a);
        //         await _repository.SaveChangesAsync();
        //     }

        //     var customerOrder = await _repository.CustomerOrders.ToListAsync();
        //     return Json(customerOrder);
        // }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetOrders()
        {
            var customerOrder = await _repository.CustomerOrders.ToListAsync();
            return Json(customerOrder);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> OrderDetails(int? id)
        {
            var cust = await _repository.CustomerOrders.FirstOrDefaultAsync(p => p.Id == id && p.Username == User.Identity.Name);
            var assorts = await _repository.CustomerOrderAssorts.Where(p => p.CustOrderId == cust.Id).ToListAsync();
            List<MyOrderAssortsView> assortlists = new List<MyOrderAssortsView>();

            ViewBag.CustId = cust.Id;
            ViewBag.IsOrderCancel = cust.IsOrderCancel;

            foreach (var a in assorts)
            {
                var unit = await _repository.ProductUnits.FindAsync(a.UnitId);
                var shade = await _repository.Shades.FindAsync(a?.ShadeId);

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

                if (shade != null && shade.IsVisiblePrice)
                {
                    orderAssortsView.Price = shade.Price;
                }
                else
                {
                    orderAssortsView.Price = unit.Price;
                }

                assortlists.Add(orderAssortsView);
            }

            return View(assortlists);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyOrders()
        {
            List<MyOrderView> mv = new List<MyOrderView>();

            var customerOrders = await _repository.CustomerOrders.Where(p => p.Username == User.Identity.Name).ToListAsync();

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

                charges = (await _repository.ShippingMasters.FirstOrDefaultAsync(p => p.Pincode == co.Pincode && p.MinAmount > netamount))?.ShippingCharge ?? 0;

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
        public async Task<ActionResult> CancelOrder(int id)
        {
            var custorder = await _repository.CustomerOrders.FindAsync(id);

            custorder.IsOrderCancel = true;
            await _repository.SaveChangesAsync();

            //need to add here a log for order status
            OrderStatus status = new OrderStatus();
            status.CustId = custorder.Id;
            status.Username = User.Identity.Name;
            status.OrdStatus = ORDSTATUS.CANCELLED.ToString();
            status.Remark = "Order has been cancelled";
            status.CreatedBy = User.Identity.Name;
            status.CreatedDate = DateTime.Now;
            _repository.OrderStatuses.Add(status);
            await _repository.SaveChangesAsync();

            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (username == null || password == null)
            {
                return NoContent();
            }

            if (await _userManager.FindByNameAsync(username) != null)
            {
                username = (await _userManager.FindByNameAsync(username)).UserName;
            }
            else if (await _userManager.FindByEmailAsync(username) != null)
            {
                username = (await _userManager.FindByEmailAsync(username)).UserName;
            }
            else if (_userManager.Users.Any(p => p.PhoneNumber == username))
            {
                username = _userManager.Users.FirstOrDefault(p => p.PhoneNumber == username).UserName;
            }
            else
            {
                ModelState.AddModelError(string.Empty, username + " user not found in our database, please sign up.");
                return View("Checkout");
            }

            //to check if user is confirmed or not
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && !user.EmailConfirmed)
            {
                return Redirect("/Account/SendEmailConfirmLink");
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect("/Home/Checkout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Checkout");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();


            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EditAddress(int? id, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            Address address = await _repository.Addresses.FirstOrDefaultAsync(p => p.Username == User.Identity.Name && p.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", address.CountryId);
            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", address.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", address.CityId);

            return View(address);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> EditAddress(Address address)
        {
            if (ModelState.IsValid)
            {

                if (!_repository.Pincodes.Any(p => Convert.ToInt32(p.Pincodes) == address.Pincode))
                {
                    ModelState.AddModelError(string.Empty, "Sorry : Delivery not available for this pincode.");
                    return View(address);
                }

                var pincode = _repository.Pincodes.FirstOrDefault(p => Convert.ToInt32(p.Pincodes) == address.Pincode);
                var city = _repository.Cities.FirstOrDefault(p => p.Id == pincode.CityId);
                var state = _repository.States.FirstOrDefault(p => p.Id == city.StateId);
                var country = _repository.Countries.FirstOrDefault(p => p.Id == state.CountryId);

                if (pincode == null || city == null || state == null || country == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Pincode");
                    return View(address);
                }

                address.CountryId = country.Id;
                address.StateId = state.Id;
                address.CityId = city.Id;
                address.Pincode = Convert.ToInt32(pincode.Pincodes);

                address.UpdatedBy = User.Identity.Name;
                address.UpdatedDate = DateTime.Now;
                address.Username = User.Identity.Name;

                _repository.Entry(address).State = EntityState.Modified;
                await _repository.SaveChangesAsync();

                return RedirectToAction("MyOrders");
            }

            ViewBag.CountryId = new SelectList(_repository.Countries, "Id", "CountryName", address.CountryId);
            ViewBag.StateId = new SelectList(_repository.States, "Id", "StateName", address.StateId);
            ViewBag.CityId = new SelectList(_repository.Cities, "Id", "CityName", address.CityId);

            return View(address);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SearchProduct()
        {
            return View();
        }

        //[Produces("application/json")]
        // [HttpGet]
        // [AllowAnonymous]
        // public async Task<IActionResult> SearchProducts()
        // {
        //     try
        //     {
        //         string term = HttpContext.Request.Query["term"].ToString();

        //         // var names = _repository.Products
        //         // .Where(p => p.IsActive && (p.ProductName.Contains(term)
        //         // // p.Brand.Contains(term) ||
        //         // // p.ItemModelNumber.Contains(term) ||
        //         // // p.ProdBarcode.Contains(term)
        //         // ))
        //         // .Select(p => new { label = p.ProductName, val = p.Id })
        //         // .ToList();

        //         var products = _repository.Products.Where(p => p.IsActive).ToList();

        //         foreach (var item in products)
        //         {
        //             var unit = _repository.ProductUnits.FirstOrDefault(p => p.ProductId == item.Id);
        //             var shade = _repository.Shades?.FirstOrDefault(p => p.ProductId == item.Id);
        //             var brand = await _repository.Brands.FindAsync(item.BrandId);

        //             item.Discription = item.Id + ";" + unit.Id + ";" + shade?.Id;
        //             item.ProductName = brand.BrandName + " " + item.ProductName;
        //         }

        //         // var names = _repository.Products
        //         // .Where(p => p.IsActive && (p.ProductName.Contains(term)))
        //         // .Select(p => new { label = p.ProductName, val = p.Id })
        //         // .ToList();

        //         var names = products
        //         .Where(p => p.ProductName.ToLower().Contains(term))
        //         .Select(p => new { label = p.ProductName, val = p.Discription })
        //         .ToList();

        //         return Ok(names);
        //     }
        //     catch
        //     {
        //         return BadRequest();
        //     }
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class Response
    {
        public string message_id { get; set; }
        public int message_count { get; set; }
        public double price { get; set; }
    }

    public class RootObject
    {
        public Response Response { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }
    }
}
