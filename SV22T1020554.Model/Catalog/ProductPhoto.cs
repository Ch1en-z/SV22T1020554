namespace SV22T1020554.Models.Catalog
{
    /// <summary>
    /// ?nh c?a m?t hàng
    /// </summary>
    public class ProductPhoto
    {
        /// <summary>
        /// M? ?nh
        /// </summary>
        public long PhotoID { get; set; }

        /// <summary>
        /// M? m?t hàng
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Tên file ?nh
        /// </summary>
        public string Photo { get; set; } = string.Empty;

        /// <summary>
        /// Mô t? ?nh
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Th? t? hi?n th? (giá tr? nh? s? hi?n th? trý?c)
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Có ?n ?nh ð?i v?i khách hàng hay không?
        /// </summary>
        public bool IsHidden { get; set; }
    }
}
