namespace SV22T1020554.Models.Partner
{
    /// <summary>
    /// Khách hàng
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// M? khách hàng
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        /// <summary>
        /// Tên giao d?ch
        /// </summary>
        public string ContactName { get; set; } = string.Empty;
        /// <summary>
        /// T?nh/thành
        /// </summary>
        public string? Province { get; set; }
        /// <summary>
        /// Ð?a ch?
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Ði?n tho?i
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Khách hàng hi?n có b? khóa hay không?
        /// </summary>
        public bool? IsLocked { get; set; }
    }
}
