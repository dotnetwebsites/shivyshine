using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;
using Shivyshine.Services;
using Shivyshine.Utilities;


namespace Shivyshine.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ApplicationDbContext _repository;
        private readonly IMailService _emailSender;
        private readonly IMemoryCache MemoryCache;
        private readonly IMapper _mapper;
        private readonly IExcel _ex;

        public ReportsController(ILogger<ReportsController> logger, ApplicationDbContext dbContext,
                                IMemoryCache memoryCache, IMailService emailSender,
                                IMapper mapper, IExcel ex)
        {
            _logger = logger;
            _repository = dbContext;
            MemoryCache = memoryCache;
            _emailSender = emailSender;
            _mapper = mapper;
            _ex = ex;
        }
        public async Task<IActionResult> AllActiveProducts()
        {
            var results = _mapper.Map<List<ExportProduct>>(await _repository.Products.ToListAsync());

            foreach (var item in results)
            {
                var prod = await _repository.Products.FindAsync(item.Id);

                item.Brand = (await _repository.Brands.FindAsync(prod.BrandId)).BrandName;
                item.Category = (await _repository.Categories.FindAsync(prod.CategoryId)).CategoryName;
                item.SubCategory = (await _repository.SubCategories.FindAsync(prod.SubCategoryId)).SubCategoryName;
                item.SuperCategory = (await _repository.SuperCategories.FindAsync(prod.SuperCategoryId)).SuperCategoryName;
            }

            return _ex.Export<ExportProduct>(results, "Product", "Product");
        }

    }
}