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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Controllers
{
    [AllowAnonymous]
    public class TNCController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public TNCController(ApplicationDbContext repository,
                                IWebHostEnvironment webHostEnvironment,
                                IMapper mapper)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("TermsAndConditions")]
        public IActionResult TermsCondition()
        {
            return View();
        }

        [Route("ShippingPolicy")]
        public IActionResult ShippingPolicy()
        {
            return View();
        }

        [Route("CancellationPolicy")]
        public IActionResult CancellationPolicy()
        {
            return View();
        }

        [Route("PrivacyPolicy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

    }
}