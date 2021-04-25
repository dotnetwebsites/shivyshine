using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Areas.Admin.Models;
using Shivyshine.Areas.Identity.Data;
using Shivyshine.Models;

namespace Shivyshine.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<POViewModel>().HasNoKey();

            builder.Entity<Brand>().HasIndex(u => u.BrandName).IsUnique();

            builder.Entity<Category>().HasIndex(u => u.CategoryName).IsUnique();
            builder.Entity<SubCategory>().HasIndex(u => u.SubCategoryName).IsUnique();
            builder.Entity<SuperCategory>().HasIndex(u => u.SuperCategoryName).IsUnique();

            builder.Entity<Country>().HasIndex(u => u.CountryName).IsUnique();
            builder.Entity<Country>().HasIndex(u => u.CountryCode).IsUnique();
            builder.Entity<Country>().HasIndex(u => u.ShortName).IsUnique();

            builder.Entity<State>().HasIndex(u => u.StateName).IsUnique();
            builder.Entity<State>().HasIndex(u => u.StateCode).IsUnique();
            builder.Entity<State>().HasIndex(u => u.ShortName).IsUnique();

            builder.Entity<City>().HasIndex(u => u.CityName).IsUnique();
            builder.Entity<City>().HasIndex(u => u.CityCode).IsUnique();
            builder.Entity<City>().HasIndex(u => u.ShortName).IsUnique();

            builder.Entity<Pincode>().HasIndex(u => u.Pincodes).IsUnique();

            builder.Entity<Product>().HasIndex(u => u.ProductName).IsUnique();
            //builder.Entity<Product>().HasIndex(u => u.HsnCode).IsUnique();

            builder.Entity<Employee>().HasIndex(u => u.Username).IsUnique();
            builder.Entity<Employee>().HasIndex(u => u.Email).IsUnique();

            builder.Entity<EmployeeRole>().HasKey(c => new { c.RoleId, c.EmpId });

            builder.Entity<ProductUnit>()
            .HasIndex(p => new { p.ProductId, p.Quantity, p.QuantityType })
            .IsUnique(true);

            builder.Entity<Shade>()
            .HasIndex(p => new { p.ProductId, p.ShadeName })
            .IsUnique(true);

            builder.Entity<SerialNoMaster>()
            .HasIndex(p => new { p.Prefix, p.Type })
            .IsUnique(true);

            builder.Entity<NumberSeries>()
            .HasIndex(p => new { p.Type, p.Prefix, p.Number })
            .IsUnique(true);

            //builder.Entity<Post>().HasOne(p => p.Blog).WithMany(b => b.Posts);            
        }

        public DbSet<DynamicMenu> DynamicMenus { get; set; }
        public DbSet<AspNetUserRoleMenu> AspNetUserRoleMenus { get; set; }
        public DbSet<MailLibrary> MailLibraries { get; set; }
        public DbSet<LogToken> LogTokens { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<SuperCategory> SuperCategories { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Pincode> Pincodes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Shade> Shades { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ShippingMaster> ShippingMasters { get; set; }
        public DbSet<NumberSeries> NumberSeries { get; set; }
        public DbSet<SerialNoMaster> SerialNoMasters { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderAssort> CustomerOrderAssorts { get; set; }
        public DbSet<ContactModel> ContactModels { get; set; }
        public DbSet<PaymentInitiateModel> PaymentInitiateModels { get; set; }
        public DbSet<OrderDelivery> OrderDeliveries { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<VendorBrand> vendorBrands { get; set; }
        public DbSet<VendorMaster> VendorMasters { get; set; }

        public DbSet<MainBanner> MainBanners { get; set; }
        public DbSet<SubBanner> SubBanners { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ERole> ERoles { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<POAssort> POAssorts { get; set; }

        [NotMapped]
        public DbSet<POViewModel> POViewModels { get; set; }
    }
}
