using System.Collections.Generic;
using System.Threading.Tasks;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Areas.Admin.Repository
{
    public interface IDynamicMenu
    {
        bool SaveChanges();
        DynamicMenu FindById(int id);
        void Add(DynamicMenu cmd);
        void Modify(DynamicMenu cmd);
        void Remove(DynamicMenu cmd);
    }
}