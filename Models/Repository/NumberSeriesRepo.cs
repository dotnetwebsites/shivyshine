using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Models
{
    public class NumberSeriesRepo : INumberSeries
    {
        private readonly ApplicationDbContext _repository;

        public NumberSeriesRepo(ApplicationDbContext repository)
        {
            _repository = repository;
        }

        public string FindOrderNumber()
        {
            throw new NotImplementedException();
        }

        public string FindOrderNumberByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public string FindOrderNumberByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public string GenerateOrderNumber(string username)
        {
            var series = _repository.SerialNoMasters.FirstOrDefault(p => p.Type == "ORDER" && p.IsActive);

            int no = 1;

            if (_repository.NumberSeries.Any(p => p.Type == series.Type && p.Prefix == series.Prefix))
                no = _repository.NumberSeries.Where(p => p.Type == series.Type &&
                p.Prefix == series.Prefix).ToList().Max(p => p.Number) + 1;

            string orderno = series.Prefix + "/" + no.ToString().PadLeft(5, '0');

            NumberSeries s = new NumberSeries();
            s.Username = username;
            s.Type = series.Type;
            s.Prefix = series.Prefix;
            s.Number = no;
            s.OrderNo = orderno;
            s.CreatedBy = username;
            s.CreatedDate = DateTime.Now;

            _repository.NumberSeries.Add(s);
            _repository.SaveChanges();


            return orderno;
        }
    }
}