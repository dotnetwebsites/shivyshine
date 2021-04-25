
using AutoMapper;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Models;

namespace Shivyshine.Utilities
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DynamicMenu, DynamicMenuView>();

            CreateMap<SubCategory, SubCategoryView>();
            CreateMap<SuperCategory, SuperCategoryView>();

            CreateMap<State, StateView>();
            CreateMap<City, CityView>();
            CreateMap<Pincode, PincodeView>();

            CreateMap<Product, ProductView>();

            CreateMap<SubBanner, SubBannerView>();

            CreateMap<VendorBrand, VendorBrandView>();
            CreateMap<PurchaseOrder, PurchaseOrderView>();



            CreateMap<Brand, ExportBrand>();
            CreateMap<Category, ExportCategory>();
            CreateMap<SubCategory, ExportSubCategory>();
            CreateMap<SuperCategory, ExportSuperCategory>();

            CreateMap<Country, ExportCountry>();
            CreateMap<State, ExportState>();
            CreateMap<City, ExportCity>();

            CreateMap<Pincode, ExportPincode>();
            CreateMap<Product, ExportProduct>();
            CreateMap<ProductUnit, ExportProductUnit>();
            CreateMap<Shade, ExportShade>();
            
        }
    }
}