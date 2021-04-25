using System;
using System.Linq;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Data;

namespace Shivyshine.Areas.Admin.Repository
{
    public class BrandRepo : IBrand
    {
        private readonly ApplicationDbContext _repository;

        public BrandRepo(ApplicationDbContext repository)
        {
            _repository = repository;
        }

        public void Add(Brand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _repository.Brands.Add(cmd);
        }

        public Brand FindById(int id)
        {
            return _repository.Brands.FirstOrDefault(p => p.Id == id);
        }

        public void Modify(Brand cmd)
        {
            //nothing
        }

        public void Remove(Brand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _repository.Brands.Remove(cmd);
        }

        public bool SaveChanges()
        {
            return (_repository.SaveChanges() >= 0);
        }
    }
}