namespace SV22T1020554.Models.Security
{
    /// <summary>
    /// Thông tin tài kho?n ngý?i dùng
    /// </summary>
    public class UserAccount
    {
        /// <summary>
        /// M? tài kho?n
        /// </summary>
        public string UserId { get; set; } = "";
        /// <summary>
        /// Tên ðãng nh?p
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// Tên hi?n th? (thý?ng là h? tên c?a ngý?i dùng, ho?c có th? là tên ðãng nh?p n?u không có h? tên)
        /// </summary>
        public string DisplayName { get; set; } = "";
        /// <summary>
        /// Ð?a ch? email (n?u có)
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// Tên fie ?nh ð?i di?n c?a ngý?i dùng (n?u có)
        /// </summary>
        public string Photo { get; set; } = "";
        /// <summary>
        /// Danh sách tên các vai tr?/quy?n c?a ngý?i dùng, ðý?c phân cách b?i d?u ch?m ph?y (n?u có)
        /// </summary>
        public string RoleNames { get; set; } = "";
    }
}
