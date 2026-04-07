namespace SV22T1020554.Models.Common
{
    /// <summary>
    /// L?p dùng ð? bi?u di?n thông tin ð?u vào c?a m?t truy v?n/t?m ki?m 
    /// d? li?u ðõn gi?n dý?i d?ng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        private const int MaxPageSize = 100; // Gi?i h?n t?i ða 100 d?ng m?i trang
        private int _page = 1;
        private int _pageSize = 20;
        private string _searchValue = "";

        /// <summary>
        /// Trang c?n ðý?c hi?n th? (b?t ð?u t? 1)
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// S? d?ng ðý?c hi?n th? trên m?i trang
        /// (0 có ngh?a là hi?n th? t?t c? các d?ng trên m?t trang, t?c là không phân trang)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 0)
                    _pageSize = 0;
                else if (value > MaxPageSize)
                    _pageSize = MaxPageSize;
                else
                    _pageSize = value;
            }
        }

        /// <summary>
        /// Giá tr? t?m ki?m (n?u có) ðý?c s? d?ng ð? l?c d? li?u 
        /// (N?u không có giá tr? t?m ki?m, h?y ð? r?ng)
        /// </summary>
        public string SearchValue
        {
            get => _searchValue;
            set => _searchValue = value?.Trim() ?? "";
        }

        /// <summary>
        /// S? d?ng c?n b? qua (tính t? d?ng ð?u tiên c?a t?p d? li?u) 
        /// ð? l?y d? li?u cho trang hi?n t?i
        /// </summary>
        public int Offset => PageSize > 0 ? (Page - 1) * PageSize : 0;
    }
}
