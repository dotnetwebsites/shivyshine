using System;
using System.Linq;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;

namespace Shivyshine.Areas.Admin.Repository
{
    public class DynamicMenuRepo : IDynamicMenu
    {
        private readonly ApplicationDbContext _repository;

        public DynamicMenuRepo(ApplicationDbContext repository)
        {
            _repository = repository;
        }

        public void Add(DynamicMenu cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _repository.DynamicMenus.Add(cmd);
        }

        public DynamicMenu FindById(int id)
        {
            return _repository.DynamicMenus.FirstOrDefault(p => p.Id == id);
        }

        public void Modify(DynamicMenu cmd)
        {
            //nothing
        }

        public void Remove(DynamicMenu cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _repository.DynamicMenus.Remove(cmd);
        }

        public bool SaveChanges()
        {
            return (_repository.SaveChanges() >= 0);
        }
    }
}