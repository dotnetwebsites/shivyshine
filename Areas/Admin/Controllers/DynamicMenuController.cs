using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SamyraGlobal.Utilities;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Data;
using Shivyshine.Models;
using Shivyshine.Utilities;

namespace Shivyshine.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "superadmin")]
    [AllowAnonymous]
    public class DynamicMenuController : Controller
    {
        private readonly ILogger<DynamicMenuController> _logger;
        private readonly ApplicationDbContext db;
        private readonly IMemoryCache cache;
        private static string[] menuId = { "2_Admin_Control", "2_Admin_Control" };
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public DynamicMenuController(ILogger<DynamicMenuController> logger,
         ApplicationDbContext dbContext,
         IMemoryCache cache,
         IMapper mapper,
         RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            db = dbContext;
            this.cache = cache;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        private async Task<IEnumerable<DynamicMenuView>> LoadAsync()
        {
            var menulists = _mapper.Map<IEnumerable<DynamicMenuView>>(await db.DynamicMenus.ToListAsync());

            foreach (var menulist in menulists)
            {
                if (menulist.ParentId != null)
                    menulist.ParentMenuName = menulists.FirstOrDefault(p => p.Id == menulist.ParentId).MenuName;
                else
                    menulist.ParentMenuName = "";
            }

            return menulists;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menulists = await this.LoadAsync();

            return View(menulists);
        }

        // GET: Grade/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Error();
            }
            DynamicMenu dynamicMenu = await db.DynamicMenus.FindAsync(id);
            if (dynamicMenu == null)
            {
                return NotFound();
            }

            ViewBag.DyId = menuId;
            return View(dynamicMenu);
        }

        // GET: Grade/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.DynamicMenus, "Id", "MenuName");
            ViewBag.Area = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "Admin", Value = "Admin"}
            }, "Value", "Text");
            ViewBag.DyId = menuId;
            return View();
        }

        // POST: Grade/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DynamicMenu dynamicMenu)
        {
            if (ModelState.IsValid)
            {
                dynamicMenu.CreatedBy = User.Identity.Name;
                dynamicMenu.CreatedDate = DateTime.Now;
                dynamicMenu.IsActive = false;
                dynamicMenu.IsAuth = false;

                db.DynamicMenus.Add(dynamicMenu);
                int result = await db.SaveChangesAsync();

                if (result > 0)
                {
                    UserMenuRoleViewModel userMenuRoleViewModel = new UserMenuRoleViewModel();
                    var superadminrole = await _roleManager.FindByNameAsync("superadmin");

                    if (superadminrole != null &&
                        !await userMenuRoleViewModel.IsInMenuRoleAsync(dynamicMenu, superadminrole.Id, db))
                        await userMenuRoleViewModel.AddToMenuRoleAsync(dynamicMenu, superadminrole.Id, db);
                }

                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.DynamicMenus, "Id", "MenuName", dynamicMenu.ParentId);
            ViewBag.Area = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "Admin", Value = "Admin"}
            }, "Value", "Text", dynamicMenu.Area);

            ViewBag.DyId = menuId;
            return View(dynamicMenu);
        }

        // GET: ProductMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DynamicMenu dynamicMenu = await db.DynamicMenus.FindAsync(id);
            if (dynamicMenu == null)
            {
                return NotFound();
            }

            ViewBag.ParentId = new SelectList(db.DynamicMenus, "Id", "MenuName", dynamicMenu.ParentId);
            ViewBag.Area = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "Admin", Value = "Admin"}
            }, "Value", "Text", dynamicMenu.Area);

            ViewBag.DyId = menuId;
            return View(dynamicMenu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DynamicMenu dynamicMenu)
        {
            if (ModelState.IsValid)
            {
                dynamicMenu.UpdatedBy = User.Identity.Name;
                dynamicMenu.UpdatedDate = DateTime.Now;

                db.Entry(dynamicMenu).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.DynamicMenus, "Id", "MenuName", dynamicMenu.ParentId);
            ViewBag.Area = new SelectList(new List<SelectListItem> {
                new SelectListItem { Text = "Admin", Value = "Admin"}
            }, "Value", "Text", dynamicMenu.Area);

            ViewBag.DyId = menuId;
            return View(dynamicMenu);
        }

        // POST: ProductMaster/Delete/5
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DynamicMenu dynamicMenu = await db.DynamicMenus.FindAsync(id);

            bool exists = db.DynamicMenus.Any(p => p.ParentId == id);

            if (!exists)
            {
                var menuroles = await db.AspNetUserRoleMenus.Where(p => p.MenuId == dynamicMenu.Id).ToListAsync();

                foreach (var menurole in menuroles)
                {
                    db.AspNetUserRoleMenus.Remove(menurole);
                    await db.SaveChangesAsync();
                }

                db.DynamicMenus.Remove(dynamicMenu);
                await db.SaveChangesAsync();


                ViewBag.DyId = menuId;
                return RedirectToAction("Index");
            }
            else
            {
                Message.Show(this,
                "You cannot delete this record!",
                "Sub menu already added with this records, please delete those records and try again.",
                MessageType.warning);

                var menulists = await this.LoadAsync();
                return View("Index", menulists);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
    
}

