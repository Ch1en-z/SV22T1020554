namespace SV22T1020554.Models.Common
{
    /// <summary>
    /// Ph?n t? trên thanh phân trang, có th? là m?t s? trang ho?c d?u "..." ð? phân cách các nhóm trang
    /// </summary>
    public class PageItem
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pageNumber">0 n?u là ph?n t? dùng ð? th? hi?n d?u "..." phân cách</param>
        /// <param name="isCurrent"></param>
        public PageItem(int pageNumber, bool isCurrent = false)
        {
            Page = pageNumber;
            IsCurrent = isCurrent;
        }

        /// <summary>
        /// S? trang (có giá tr? là 0 n?u là d?u "..." ð? phân cách các nhóm trang)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Có ph?i là trang hi?n t?i hay không?
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Có ph?i là v? trí hi?n th? d?u "..." ð? phân cách các nhóm trang hay không?
        /// </summary>
        public bool IsEllipsis => Page == 0;
    }
}
