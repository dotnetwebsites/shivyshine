using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    public class LayoutViewController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public LayoutViewController(ApplicationDbContext repository,
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

        [HttpGet]
        public JsonResult LoadCategory()
        {
            LayoutViewModel model = new LayoutViewModel();

            model.Categories = _repository.Categories.Take(8).ToList();
            model.SubCategories = _repository.SubCategories.ToList();
            model.SuperCategories = _repository.SuperCategories.OrderBy(p=>p.SuperCategoryName).ToList();

            return Json(model);
        }

        [HttpGet]
        public JsonResult LoadSubCategory(int id)
        {
            var subcategory = _repository.SubCategories.Where(p => p.CategoryId == id).ToList();
            return Json(subcategory);
        }

        [HttpGet]
        public JsonResult LoadSuperCategory(int id)
        {
            var supercategory = _repository.SuperCategories.Where(p => p.SubCategoryId == id).OrderBy(p=>p.SuperCategoryName).ToList();
            return Json(supercategory);
        }

    }
}