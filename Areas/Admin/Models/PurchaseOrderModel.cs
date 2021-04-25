using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class POViewModel
    {
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int ShadeId { get; set; }
        public double MRP { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }

    }

    public class POSaveModel
    {
        public DateTime PODate { get; set; }
        public string Assorts { get; set; }
        // public List<int> UnitId { get; set; }
        // public List<int> ShadeId { get; set; }
        // public List<int> Quantity { get; set; }
        public int VendorId { get; set; }
    }

    public class PurchaseOrder : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime PODate { get; set; }
        public int VendorId { get; set; }
    }

    public class PurchaseOrderView : PurchaseOrder
    {
        public string VendorName { get; set; }
    }

    public class POAssort : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public int POId { get; set; }
        public int BrandId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int ShadeId { get; set; }
        public int Quantity { get; set; }
    }
}