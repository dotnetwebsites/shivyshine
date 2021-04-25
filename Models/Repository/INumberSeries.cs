using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public interface INumberSeries
    {
        string FindOrderNumber();
        string FindOrderNumberByDate(DateTime date);
        string FindOrderNumberByUserName(string username);
        string GenerateOrderNumber(string username);

    }
}