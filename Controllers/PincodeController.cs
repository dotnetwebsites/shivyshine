using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Shivyshine.Data;

namespace Shivyshine.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class PincodeController : Controller
    {
        private readonly ApplicationDbContext _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PincodeController(ApplicationDbContext repository,
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

        public async Task<IActionResult> Search(int? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            
            var pincode = await _repository.Pincodes.FindAsync(id);
            return View(pincode);
        }

    }
}