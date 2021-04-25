using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;
using Microsoft.Extensions.Logging;

namespace Shivyshine.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger,
                                ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper,
                                IHttpContextAccessor httpContextAccessor,
                                IConfiguration configuration,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var PaymentInitiateModel = await _repository.PaymentInitiateModels.FindAsync(id);
            return View(PaymentInitiateModel);
        }

        public async Task<IActionResult> Razorpay(Guid id)
        {
            var PaymentInitiateModel = await _repository.PaymentInitiateModels.FindAsync(id);
            return View(PaymentInitiateModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(PaymentInitiateModel paymentData)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(10000000, 100000000).ToString();

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(_configuration["key"], _configuration["secret"]);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", paymentData.Amount * 100);  // Amount will in paise
            options.Add("receipt", transactionId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0"); // 1 - automatic  , 2 - manual
                                                 //options.Add("notes", "-- You can put any notes here --");
            Razorpay.Api.Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            // Create order model for return on view
            OrderModel orderModel = new OrderModel
            {
                orderId = orderResponse.Attributes["id"],
                razorpayKey = _configuration["key"],
                amount = paymentData.Amount * 100,
                currency = "INR",
                name = paymentData.Name,
                email = paymentData.Email,
                contactNumber = paymentData.ContactNumber,
                address = paymentData.Address,
                description = "Payment Initiate"
            };

            PaymentInitiateModel model = await _repository.PaymentInitiateModels.FindAsync(paymentData.Id);

            model.OrderId = orderModel.orderId;
            model.ConfirmAmount = orderModel.amount;
            model.Currency = orderModel.currency;

            await _repository.SaveChangesAsync();

            // Return on PaymentPage with Order data
            return View("PaymentPage", orderModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Complete()
        {
            // Payment data comes in url so we have to get it from url

            // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
            string paymentId = Request.Form["rzp_paymentid"];

            // This is orderId
            string orderId = Request.Form["rzp_orderid"];

            if (paymentId == null || orderId == null)
            {
                ModelState.AddModelError(string.Empty, "Paymentid or orderid has been null");
                return RedirectToAction("PaymentPage");
            }

            Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(_configuration["key"], _configuration["secret"]);

            Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
            string amt = paymentCaptured.Attributes["amount"];

            //// Check payment made successfully

            if (paymentCaptured.Attributes["status"] == "captured")
            {
                //_logger.LogInformation("Payment Captured");
                //_logger.LogInformation("Payment Id" + paymentId);
                //_logger.LogInformation("Order Id" + orderId);
                //_logger.LogInformation("success");

                PaymentInitiateModel model = _repository.PaymentInitiateModels.FirstOrDefault(p => p.OrderId == orderId);                
                model.PaymentId = paymentId;
                model.Status = "success";
                await _repository.SaveChangesAsync();

                // Create these action method
                return RedirectToAction("Success");
            }
            else
            {            
                PaymentInitiateModel model = _repository.PaymentInitiateModels.FirstOrDefault(p => p.OrderId == orderId);                
                model.Status = "failed";
                await _repository.SaveChangesAsync();
                
                return RedirectToAction("Failed");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success()
        {
            if (Request.Cookies["payid"] != null)
            {
                //IEnumerable<StudentViewModel> students = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://mysms.msg24.in/api/mt/GetDelivery?user=SHIVYSHINELLP&password=123456&jobid=7515412");
                    //HTTP GET
                    var responseTask = client.GetAsync("student");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    // if (result.IsSuccessStatusCode)
                    // {
                    //     var readTask = result.Content.ReadAsAsync<IList<StudentViewModel>>();
                    //     readTask.Wait();

                    //     students = readTask.Result;
                    // }
                    // else //web api sent error response 
                    // {
                    //     //log response status here..

                    //     students = Enumerable.Empty<StudentViewModel>();

                    //     ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    // }
                }

                var user = await _userManager.GetUserAsync(User);
                //  int cid = Convert.ToInt32(Request.Cookies["payid"].ToString());

                //  var custord = await _repository.CustomerOrders.FindAsync(cid);

                // SendSms sms = new SendSms(custord.OrderNo, custord.OrderDate);

                // var sURL = _configuration["SmsUrl"].ToString() +
                //         "APIKey=" + _configuration["SMSApi"].ToString() +
                //         "&senderid=" + _configuration["SenderName"].ToString() +
                //         "&channel=" + _configuration["SMSChnl"].ToString() +
                //         "&DCS=" + _configuration["SMSDCS"].ToString() +
                //         "&flashsms=" + _configuration["SMSFlash"].ToString() +
                //         "&number=" + user.PhoneNumber +
                //         "&text=" + sms.Message +
                //         "&route=" + _configuration["SmsRoute"].ToString();

                // try
                // {
                //     using (WebClient client = new WebClient())
                //     {

                //         string s = client.DownloadString(sURL);

                //         var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(s);
                //         int n = responseObject.Status;
                //         // if (n == 3)
                //         // {
                //         //     Json("Message does not Send Successfully due to invalid credentials");
                //         // }
                //         // else
                //         // {
                //         //     Json("Message Send Successfully !");                        
                //         // }
                //     }
                // }
                // catch (Exception ex)
                // {
                //     ModelState.AddModelError("", "Error in sending Message");
                //     ex.ToString();
                // }
            }

            return View();
        }


        [HttpGet]
        public IActionResult ReturnStatusofMessage()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mysms.msg24.in/api/mt/GetDelivery?user=SHIVYSHINELLP&password=123456&jobid=7515412");
                //HTTP GET
                var responseTask = client.GetAsync("student");
                responseTask.Wait();

                var result = responseTask.Result;
            }

            return View();
        }

        [HttpGet]
        [Route("/cashondeliver/reciept")]
        public IActionResult CODSuccess()
        {
            return View("Success");
        }

        [HttpGet]
        public IActionResult Failed()
        {
            return View();
        }

        public class OrderModel
        {
            public string orderId { get; set; }
            public string razorpayKey { get; set; }
            public double amount { get; set; }
            public string currency { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string contactNumber { get; set; }
            public string address { get; set; }
            public string description { get; set; }
        }

    }
}