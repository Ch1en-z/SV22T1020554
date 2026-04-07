using SV22T1020554.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1020554.Models.Catalog
{
    /// <summary>
    /// Bi?u di?n d? li?u ð?u vào t?m ki?m, phân trang ð?i v?i m?t hàng
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// M? lo?i hàng (0 n?u b? qua)
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// M? nhà cung c?p (0 n?u b? qua)
        /// </summary>
        public int SupplierID { get; set; }

        /// <summary>
        /// Giá t?i thi?u (0 n?u b? qua)
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// M?c giá t?i ða (0 n?u b? qua)
        /// </summary>
        public decimal MaxPrice { get; set; }
    }
}
