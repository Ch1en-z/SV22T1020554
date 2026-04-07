namespace SV22T1020554.Models.Sales
{
    /// <summary>
    /// DTO hi?n th? thông tin chi ti?t c?a m?t hàng trong ðõn hàng
    /// </summary>
    public class OrderDetailViewInfo : OrderDetail
    {
        /// <summary>
        /// Tên hàng
        /// </summary>
        public string ProductName { get; set; } = "";
        /// <summary>
        /// Ðõn v? tính
        /// </summary>
        public string Unit { get; set; } = "";
        /// <summary>
        /// Tên file ?nh
        /// </summary>
        public string Photo { get; set; } = "";
    }
}
