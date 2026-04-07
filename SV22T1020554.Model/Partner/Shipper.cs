namespace SV22T1020554.Models.Partner
{
    /// <summary>
    /// Ngý?i giao hàng
    /// </summary>
    public class Shipper
    {
        /// <summary>
        /// M? ngý?i giao hàng
        /// </summary>
        public int ShipperID { get; set; }
        /// <summary>
        /// Tên ngý?i giao hàng
        /// </summary>
        public string ShipperName { get; set; } = string.Empty;
        /// <summary>
        /// Ði?n tho?i
        /// </summary>
        public string? Phone { get; set; }
    }
}
