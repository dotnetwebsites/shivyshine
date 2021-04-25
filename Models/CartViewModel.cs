namespace Shivyshine.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public int ShadeId { get; set; }
        public int Quantity { get; set; }

        public int Pincode { get; set; }
        public double NetPay { get; set; }
    }
}