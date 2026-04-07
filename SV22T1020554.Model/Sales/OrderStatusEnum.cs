namespace SV22T1020554.Models.Sales
{
    /// <summary>
    /// Ð?nh ngh?a các tr?ng thái c?a ðõn hàng
    /// </summary>
    public enum OrderStatusEnum
    {
        /// <summary>
        /// Ðõn hàng b? t? ch?i
        /// </summary>
        Rejected = -2,

        /// <summary>
        /// Ðõn hàng b? h?y
        /// </summary>
        Cancelled = -1,

        /// <summary>
        /// Ðõn hàng v?a ðý?c t?o, chýa ðý?c x? l?
        /// </summary>
        New = 1,

        /// <summary>
        /// Ðõn hàng ð? ðý?c duy?t ch?p nh?n
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// Ðõn hàng ðang ðý?c giao cho ngý?i giao hàng ð? v?n chuy?n ð?n khách hàng
        /// </summary>
        Shipping = 3,

        /// <summary>
        /// Ðõn hàng ð? hoàn t?t (thành công)
        /// </summary>
        Completed = 4
    }
}
