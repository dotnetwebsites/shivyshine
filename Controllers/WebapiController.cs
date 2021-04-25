using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Services;
using Shivyshine.Utilities;


namespace Shivyshine.Controllers
{
    [AllowAnonymous]
    public class WebApiController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ApplicationDbContext _repository;

        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMailService _emailSender;
        private readonly IMapper _mapper;

        public WebApiController(SignInManager<ApplicationUser> signInManager,
                            UserManager<ApplicationUser> userManager,
                            ApplicationDbContext repository,
                            IWebHostEnvironment hostEnvironment,
                            IMailService emailSender,
                            IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            webHostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            _repository = repository;
            _mapper = mapper;
        }

        //[Route("webapi/search")]
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();

                // var names = _repository.Products
                // .Where(p => p.IsActive && (p.ProductName.Contains(term)
                // // p.Brand.Contains(term) ||
                // // p.ItemModelNumber.Contains(term) ||
                // // p.ProdBarcode.Contains(term)
                // ))
                // .Select(p => new { label = p.ProductName, val = p.Id })
                // .ToList();

                var products = _repository.Products.Where(p => p.IsActive).ToList();

                foreach (var item in products)
                {
                    var unit = _repository.ProductUnits.FirstOrDefault(p => p.ProductId == item.Id);
                    var shade = _repository.Shades?.FirstOrDefault(p => p.ProductId == item.Id);
                    var brand = await _repository.Brands.FindAsync(item.BrandId);

                    item.Discription = item?.Id + ";" + unit?.Id + ";" + shade?.Id;
                    item.ProductName = brand?.BrandName + " " + item.ProductName;
                }

                // var names = _repository.Products
                // .Where(p => p.IsActive && (p.ProductName.Contains(term)))
                // .Select(p => new { label = p.ProductName, val = p.Id })
                // .ToList();

                var names = products
                .Where(p => p.ProductName.ToLower().Contains(term))
                .Select(p => new { label = p.ProductName, val = p.Discription })
                .ToList();

                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewRatings(
           int pid, int uid, int sid,
           int yourRate, string name, string email, string comment)
        {
            var msg = "";

            if (!(_repository.Products.Any(p => p.Id == pid)))
                msg = "Product is invalid";

            if (!(_repository.ProductUnits.Any(p => p.Id == uid)))
                msg = "Product unit is invalid";

            if (!(_repository.Shades.Any(p => p.Id == sid)))
                msg = "shades is invalid";

            if (name == null || name == "")
                ModelState.AddModelError(string.Empty, "Please enter name");

            if (email == null || email == "")
                ModelState.AddModelError(string.Empty, "Please enter email");

            if (comment == null || comment == "")
                ModelState.AddModelError(string.Empty, "Please enter review text");

            if (!(yourRate >= 1 && yourRate <= 5))
                ModelState.AddModelError(string.Empty, "Please select rating");

            if (msg != "")
                return Json(msg);

            ProductReview productReview = new ProductReview();
            productReview.ProductId = pid;
            productReview.UnitId = uid;
            productReview.ShadeId = sid;
            productReview.Name = name;
            productReview.Email = email;
            productReview.ReviewText = comment;
            productReview.Rating = yourRate;
            productReview.CreatedBy = User.Identity.Name;
            productReview.CreatedDate = DateTime.Now;

            _repository.ProductReviews.Add(productReview);
            await _repository.SaveChangesAsync();

            return Ok(productReview);
        }


        [HttpPost]
        public async Task<IActionResult> ReviewRating([FromBody] ProductReviewView pr)
        {
            if (!(_repository.Products.Any(p => p.Id == pr.ProductId)))
                ModelState.AddModelError(string.Empty, "Product is invalid");

            if (!(_repository.ProductUnits.Any(p => p.Id == pr.UnitId)))
                ModelState.AddModelError(string.Empty, "Product unit is invalid");

            if (!(_repository.Shades.Any(p => p.Id == pr.ShadeId)))
                ModelState.AddModelError(string.Empty, "shades is invalid");

            if (pr.Name == null || pr.Name == "")
                ModelState.AddModelError(string.Empty, "Please enter name");

            if (pr.Email == null || pr.Email == "")
                ModelState.AddModelError(string.Empty, "Please enter email");

            if (pr.ReviewText == null || pr.ReviewText == "")
                ModelState.AddModelError(string.Empty, "Please enter review text");

            if (!(pr.Rating >= 1 && pr.Rating <= 5))
                ModelState.AddModelError(string.Empty, "Please select rating");

            ProductReview productReview = new ProductReview();
            productReview.ProductId = pr.ProductId;
            productReview.UnitId = pr.UnitId;
            productReview.ShadeId = pr.ShadeId;
            productReview.Name = pr.Name;
            productReview.Email = pr.Email;
            productReview.ReviewText = pr.ReviewText;
            productReview.Rating = pr.Rating;
            productReview.CreatedBy = User.Identity.Name;
            productReview.CreatedDate = DateTime.Now;

            _repository.ProductReviews.Add(productReview);
            await _repository.SaveChangesAsync();

            return Ok(productReview);

            // var errorList = (from item in ModelState.Values
            //                  from error in item.Errors
            //                  select error.ErrorMessage).ToList();
            // return NotFound(errorList);         
        }


    }
}