using System.Collections.Generic;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class LayoutViewModel
    {
        public List<Category> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
        public List<SuperCategory> SuperCategories { get; set; }
    }
}