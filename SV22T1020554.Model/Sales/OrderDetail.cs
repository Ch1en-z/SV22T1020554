namespace SV22T1020554.Models.Sales
{
    /// <summary>
    /// Thông tin chi ti?t c?a m?t hàng ðý?c bán trong ðõn hàng
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// M? ðõn hàng
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// M? m?t hàng
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// S? lý?ng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// T?ng s? ti?n
        /// </summary>
        public decimal TotalPrice => Quantity * SalePrice;        
    }
}
