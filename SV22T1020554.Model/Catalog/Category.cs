namespace SV22T1020554.Models.Catalog
{
    /// <summary>
    /// Lo?i hàng
    /// </summary>
    public class Category
    {
        /// <summary>
        /// M? lo?i hàng
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Tên lo?i hàng
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Mô t? lo?i hàng
        /// </summary>
        public string? Description { get; set; }
    }
}
