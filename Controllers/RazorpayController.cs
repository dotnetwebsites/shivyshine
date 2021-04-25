using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class RazorpayController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public IConfiguration _configuration;

        public RazorpayController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper,
                                IHttpContextAccessor httpContextAccessor,
                                IConfiguration configuration)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(PaymentInitiateModel paymentData)
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

            // Return on PaymentPage with Order data
            return View("PaymentPage", orderModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete()
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
                // Create these action method
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Failed");
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
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