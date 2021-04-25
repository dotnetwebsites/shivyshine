using System.Collections.Generic;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class ProductImageModel
    {
        public Product Product { get; set; }
        public ProductImage ProductImage { get; set; }
    }

    public class ProductImagesModel
    {
        public Product Product { get; set; }
        public List<ProductImage> ProductImage { get; set; }
    }

    public class ProductShadesImagesModel
    {
        public Product Product { get; set; }
        //public List<ProductUnit> ProductUnits { get; set; }
        public ProductUnit ProductUnit { get; set; }
        public List<ProductImage> UnitImages { get; set; }
        //public List<Shade> Shades { get; set; }
        public Shade Shade { get; set; }
        public List<ProductImage> ShadeImages { get; set; }

        public ProductReview ProductReview { get; set; }

    }

    public class ProductsImagesModel
    {
        public List<Product> Products { get; set; }
        public List<ProductImage> ProductImage { get; set; }
    }
}