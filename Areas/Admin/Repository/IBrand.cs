using System.Collections.Generic;
using System.Threading.Tasks;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Areas.Admin.Repository
{
    public interface IBrand
    {
        bool SaveChanges();
        Brand FindById(int id);
        void Add(Brand cmd);
        void Modify(Brand cmd);
        void Remove(Brand cmd);
    }
}