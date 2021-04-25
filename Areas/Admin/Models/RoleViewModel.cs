using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Data;

namespace Shivyshine.Areas.Admin.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }

    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class UserMenuRoleViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsSelected { get; set; }

        public virtual async Task<bool> IsInMenuRoleAsync(DynamicMenu menu, string role, ApplicationDbContext db)
        {
            return await db.AspNetUserRoleMenus.AnyAsync(p => p.MenuId == menu.Id && p.RoleId == role);
        }

        public virtual async Task<int> DeleteByRoleIdAsync(string RoleId, ApplicationDbContext db)
        {
            int count = 0;
            var menus = await db.AspNetUserRoleMenus.Where(p => p.RoleId == RoleId).ToListAsync();
            foreach (var menu in menus)
            {
                db.AspNetUserRoleMenus.Remove(menu);
                await db.SaveChangesAsync();
                count++;
            }
            return count;
        }


        public virtual async Task<bool> AddToMenuRoleAsync(DynamicMenu menu, string role, ApplicationDbContext db)
        {
            if (menu == null || role == null)
                return false;

            var aspNetUserRoleMenu = new AspNetUserRoleMenu
            {
                MenuId = menu.Id,
                RoleId = role
            };

            db.AspNetUserRoleMenus.Add(aspNetUserRoleMenu);

            if (await db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }

        public virtual async Task<bool> RemoveFromMenuRoleAsync(DynamicMenu menu, string role, ApplicationDbContext db)
        {
            if (menu == null || role == null)
                return false;

            var aspNetUserRoleMenu = new AspNetUserRoleMenu
            {
                MenuId = menu.Id,
                RoleId = role
            };

            db.AspNetUserRoleMenus.Remove(aspNetUserRoleMenu);

            if (await db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }


    }
}