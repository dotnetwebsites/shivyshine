using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Services;

namespace SamyraGlobal.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ApplicationDbContext db;
        private readonly IMailService _emailSender;
        private readonly IMemoryCache MemoryCache;
        public string ActiveMenu { get; } = "18_Contact_Us";

        public ContactController(ILogger<ContactController> logger, ApplicationDbContext dbContext,
                                IMemoryCache memoryCache, IMailService emailSender)
        {
            _logger = logger;
            db = dbContext;
            MemoryCache = memoryCache;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["ActiveMenu"] = ActiveMenu;

            ContactUSView viewModel = new ContactUSView();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactUSView contactUSView)
        {
            if (ModelState.IsValid)
            {
                if (contactUSView.Email == "info@shivyshine.com;")
                {
                    ModelState.AddModelError(string.Empty, "Sorry you cannot use this email, please try another one.");
                    return View(contactUSView);
                }

                contactUSView.CreatedBy = User.Identity.Name;
                contactUSView.CreatedDate = DateTime.Now;
                contactUSView.Status = 1;

                _emailSender.SendEmail("info@shivyshine.com;", "Contact US", "Contact Us : " + contactUSView.Subject,
                string.Format(@"<h3><div>Full Name : {0}</div><div>User Email : {1}</div><div>Message : {2}</div></h3>",
                                contactUSView.Name, contactUSView.Email, contactUSView.Message));

                await db.ContactModels.AddAsync(contactUSView);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(contactUSView);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}