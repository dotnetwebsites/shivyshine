using System.Collections.Generic;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
        public List<SuperCategory> SuperCategories { get; set; }
        public List<Product> Products { get; set; }
        public ProductUnit ProductUnit { get; set; }
        public Shade Shade { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<Brand> Brands { get; set; }

        public List<MainBanner> MainBanners { get; set; }
        public List<SubBanner> SubBanners { get; set; }
    }
}