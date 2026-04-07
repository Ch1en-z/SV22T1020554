namespace SV22T1020554.Shop.Models
{
    /// <summary>
    /// Mặt hàng trong giỏ hàng
    /// </summary>
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = "";
        public string Size { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal TotalPrice => Price * Quantity;
    }
}
