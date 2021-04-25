
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class CustomerOrder : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int Pincode { get; set; }
        public int AddressId { get; set; }
        public int ShippingId { get; set; }
        public double ShippingCharges { get; set; }

        public string PaymentMode { get; set; }
        public Guid PaymentId { get; set; }
        //public int OrderStatusId { get; set; }
        //public int DeliveryStatusId { get; set; }
        public bool IsOrderCancel { get; set; }
    }

    public class CustomerOrderAssort
    {
        [Key]
        public int Id { get; set; }
        public int CustOrderId { get; set; }
        public int ProductId { get; set; }
        public int UnitId { get; set; }
        public int? ShadeId { get; set; }
        public int Quantity { get; set; }
    }

    public class MyOrderView
    {
        public int CustId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public double NetPayable { get; set; }
        public int NoOfItems { get; set; }
        public string DeliveryStatus { get; set; }

        public string PayMode { get; set; }
        public Guid PayId { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsOrderCancel { get; set; }
        
    }

    public class MyOrderAssortsView
    {
        public int CustId { get; set; }
        public string ProdImgUrl { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}