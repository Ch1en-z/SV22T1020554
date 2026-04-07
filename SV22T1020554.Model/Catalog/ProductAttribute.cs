namespace SV22T1020554.Models.Catalog
{
    /// <summary>
    /// Thu?c tính c?a m?t hàng
    /// </summary>
    public class ProductAttribute
    {
        /// <summary>
        /// M? thu?c tính
        /// </summary>
        public long AttributeID { get; set; }

        /// <summary>
        /// M? m?t hàng
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Tên thu?c tính (ví d?: "Màu s?c", "Kích thý?c", "Ch?t li?u", ...)
        /// </summary>
        public string AttributeName { get; set; } = string.Empty;

        /// <summary>
        /// Giá tr? thu?c tính
        /// </summary>
        public string AttributeValue { get; set; } = string.Empty;

        /// <summary>
        /// Th? t? hi?n th? thu?c tính (giá tr? nh? s? hi?n th? trý?c)
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
