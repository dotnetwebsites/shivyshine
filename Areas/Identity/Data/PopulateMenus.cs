using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Areas.Identity.Data
{
    public class PopulateMenus
    {
        private readonly ApplicationDbContext db;
        private readonly string UserName;
        private StringBuilder sb = new StringBuilder();
        public PopulateMenus(ApplicationDbContext _db)
        {
            this.db = _db;
        }

        public PopulateMenus(ApplicationDbContext _db, string username)
        {
            this.db = _db;
            this.UserName = username;
        }

        public string MenuText()
        {
            sb.Clear();

            string query = "";

            if (UserName != null)
            {
                query = @"select distinct d.Id
                            from dynamicmenus d
                                left join AspNetUserRoleMenus rm on rm.menuid=d.id
                                left join AspNetUserRoles rs on rs.RoleId=rm.RoleId
                                left join AspNetUsers u on u.id=rs.userid
                            where (IsActive=0 and u.username='" + UserName + "') or isauth=1";
            }
            else
            {
                query = "select Id from dynamicMenus WHERE IsAuth=1 and IsActive=0";
            }

            var menuIds = db.DynamicMenus.FromSqlRaw(query).Select(p => p.Id).ToArray();

            var menus = db.DynamicMenus.Where(
                p => p.ParentId == null && p.IsActive == false && menuIds.Contains(p.Id)
            ).OrderBy(p => p.ParentId).OrderBy(p => p.MenuOrder).ToList();

            foreach (var menu in menus)
            {
                var submenus = db.DynamicMenus.Where(
                    p => p.ParentId == menu.Id && p.IsActive == false && menuIds.Contains(p.Id)
                ).OrderBy(p => p.MenuOrder).ToList();

                if (submenus.Any())
                {
                    sb.Append("<li id='" + menu.Id + "_" + menu.MenuName.Replace(" ", "_") + "' class='megamenu-holder content-none'><a href='javascript:;'>" + menu.MenuName + "</a><ul class='hb-megamenu'>");

                    foreach (var submenu in submenus)
                    {
                        var childmenus = db.DynamicMenus.Where(
                            p => p.ParentId == submenu.Id && p.IsActive == false && menuIds.Contains(p.Id)
                            ).OrderBy(p => p.MenuOrder).ToList();

                        sb.Append("<li id='" + submenu.Id + "_" + submenu.MenuName.Replace(" ", "_") + "'><a href='javascript:;'>" + submenu.MenuName + "</a>");

                        if (childmenus.Any())
                        {
                            sb.Append("<ul>");

                            foreach (var childmenu in childmenus)
                            {
                                sb.Append("<li id='" + childmenu.Id + "_" + childmenu.MenuName.Replace(" ", "_") + "'><a href='" + childmenu.MenuUrl + "'>" + childmenu.MenuName + "</a></li>");
                            }

                            sb.Append("</ul>");
                        }
                        else
                        {
                            sb.Append("<ul><li id='" + submenu.Id + "_" + submenu.MenuName.Replace(" ", "_") + "'><a href='" + submenu.MenuUrl + "'>" + submenu.MenuName + "</a></li></ul>");
                        }

                        sb.Append("</li>");

                    }
                    sb.Append("</ul></li>");
                }
                else
                {
                    sb.Append(" <li id='" + menu.Id + "_" + menu.MenuName.Replace(" ", "_") + "' class='content-none'><a href='" + menu.MenuUrl + "'>" + menu.MenuName + "</a></li>");
                }

            }

            return sb.ToString();

        }

    }
}
