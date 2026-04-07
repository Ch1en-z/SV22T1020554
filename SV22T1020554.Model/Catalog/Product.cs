namespace SV22T1020554.Models.Catalog
{
    /// <summary>
    /// M?t hàng
    /// </summary>
    public class Product
    {
        /// <summary>
        /// M? m?t hàng
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Tên m?t hàng
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Mô t? m?t hàng
        /// </summary>
        public string? ProductDescription { get; set; }

        /// <summary>
        /// M? nhà cung c?p
        /// </summary>
        public int? SupplierID { get; set; }

        /// <summary>
        /// M? lo?i hàng
        /// </summary>
        public int? CategoryID { get; set; }

        /// <summary>
        /// Ðõn v? tính
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Giá
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Tên file ?nh ð?i di?n c?a m?t hàng (n?u có)
        /// </summary>
        public string? Photo { get; set; }

        /// <summary>
        /// M?t hàng hi?n có ðang ðý?c bán hay không?
        /// </summary>
        public bool IsSelling { get; set; }
    }
}
