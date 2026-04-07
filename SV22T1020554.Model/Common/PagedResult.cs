namespace SV22T1020554.Models.Common
{
    /// <summary>
    /// L?p dùng ð? bi?u di?n k?t qu? truy v?n/t?m ki?m d? li?u dý?i d?ng phân trang
    /// </summary>
    /// <typeparam name="T">Ki?u c?a d? li?u truy v?n ðý?c</typeparam>
    public class PagedResult<T> where T : class
    {
        /// <summary>
        /// Trang ðang ðý?c hi?n th?
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// S? d?ng ðý?c hi?n th? trên m?i trang (0 có ngh?a là hi?n th? t?t c? các d?ng trên m?t trang/không phân trang)
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// T?ng s? d?ng d? li?u ðý?c t?m th?y
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Danh sách các d?ng d? li?u ðý?c hi?n th? trên trang hi?n t?i
        /// </summary>
        public List<T> DataItems { get; set; } = new List<T>();

        /// <summary>
        /// T?ng s? trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                return (int)Math.Ceiling((decimal)RowCount / PageSize);
            }
        }

        /// <summary>
        /// Có trang trý?c không?
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Có trang sau không?
        /// </summary>
        public bool HasNextPage => Page < PageCount;

        /// <summary>
        /// L?y danh sách các trang ðý?c hi?n th? trên thanh phân trang
        /// </summary>
        /// <param name="n">S? lý?ng trang lân c?n trang hi?n t?i c?n ðý?c hi?n th?</param>
        /// <returns></returns>
        public List<PageItem> GetDisplayPages(int n = 5)
        {
            var result = new List<PageItem>();

            if (PageCount <= 0)
                return result;

            n = n > 0 ? n : 5; // Giá tr? n không h?p l?, ð?t l?i v? m?c ð?nh            

            int currentPage = Page;
            if (currentPage < 1)
                currentPage = 1;
            else if (currentPage > PageCount)
                currentPage = PageCount;

            int displayedPages = 2 * n + 1;     // S? lý?ng trang t?i ða hi?n th? trên thanh phân trang (bao g?m c? trang hi?n t?i)
            int startPage = currentPage - n;    // Trang b?t ð?u hi?n th?
            int endPage = currentPage + n;      // Trang k?t thúc hi?n th?

            // N?u thi?u bên trái
            if (startPage < 1)
            {
                endPage += (1 - startPage);
                startPage = 1;
            }

            // N?u thi?u bên ph?i
            if (endPage > PageCount)
            {
                startPage -= (endPage - PageCount);
                endPage = PageCount;
            }

            // Gán l?i b?ng 1 n?u startPage b? âm sau khi tr?
            if (startPage < 1)
                startPage = 1;

            // Ð?m b?o không vý?t quá displayedPages
            if (endPage - startPage + 1 > displayedPages)
                endPage = startPage + displayedPages - 1;

            // Trang ð?u
            if (startPage > 1)
            {
                result.Add(new PageItem(1, currentPage == 1));
                // Thêm d?u "..." ð? phân cách n?u có nhi?u trang ? gi?a
                if (startPage > 2)
                    result.Add(new PageItem(0));
            }

            // Trang hi?n t?i và các trang lân c?n
            for (int i = startPage; i <= endPage; i++)
            {
                if (i <= PageCount)
                    result.Add(new PageItem(i, i == currentPage));
            }

            // Trang cu?i
            if (endPage < PageCount)
            {
                // Thêm d?u "..." ð? phân cách n?u có nhi?u trang ? gi?a
                if (endPage < PageCount - 1)
                    result.Add(new PageItem(0));
                result.Add(new PageItem(PageCount, currentPage == PageCount));
            }

            return result;
        }
    }
}
