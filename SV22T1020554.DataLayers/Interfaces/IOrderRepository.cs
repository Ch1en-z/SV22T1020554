using SV22T1020554.Models.Common;
using SV22T1020554.Models.Sales;

namespace SV22T1020554.DataLayers.Interfaces
{
    /// <summary>
    /// ??nh ngh?a c?c ch?c n?ng x? l? d? li?u cho ??n h?ng
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// T?m ki?m v? l?y danh s?ch ??n h?ng d??i d?ng ph?n trang
        /// </summary>
        Task<PagedResult<OrderViewInfo>> ListAsync(OrderSearchInput input);

        /// <summary>
        /// L?y th?ng tin 1 ??n h?ng
        /// </summary>
        Task<OrderViewInfo?> GetAsync(int orderID);

        /// <summary>
        /// B? sung ??n h?ng
        /// </summary>
        Task<int> AddAsync(Order data);

        /// <summary>
        /// C?p nh?t ??n h?ng
        /// </summary>
        Task<bool> UpdateAsync(Order data);

        /// <summary>
        /// X?a ??n h?ng
        /// </summary>
        Task<bool> DeleteAsync(int orderID);

        /// <summary>
        /// L?y danh s?ch m?t h?ng trong ??n h?ng
        /// </summary>
        Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID);

        /// <summary>
        /// L?y th?ng tin chi ti?t c?a m?t m?t h?ng trong m?t ??n h?ng
        /// </summary>
        Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID);

        /// <summary>
        /// B? sung m?t h?ng v?o ??n h?ng
        /// </summary>
        Task<bool> AddDetailAsync(OrderDetail data);

        /// <summary>
        /// C?p nh?t s? l??ng v? gi? b?n c?a m?t m?t h?ng trong ??n h?ng
        /// </summary>
        Task<bool> UpdateDetailAsync(OrderDetail data);

        /// <summary>
        /// X?a m?t m?t h?ng kh?i ??n h?ng
        /// </summary>
        Task<bool> DeleteDetailAsync(int orderID, int productID);

        /// <summary>
        /// L?y t?ng s? ??n h?ng
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// L?y doanh thu h?m nay
        /// </summary>
        Task<decimal> GetTodayRevenueAsync();

        /// <summary>
        /// L?y doanh thu theo th?ng (12 th?ng g?n nh?t)
        /// </summary>
        Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync();

        /// <summary>
        /// L?y danh s?ch ??n h?ng c?n x? l? (tr?ng th?i New ho?c Accepted)
        /// </summary>
        Task<List<OrderViewInfo>> GetPendingOrdersAsync(int top = 5);
    }

    /// <summary>
    /// D? li?u doanh thu theo th?ng
    /// </summary>
    public class MonthlyRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public string MonthName => $"Th?ng {Month}/{Year}";
    }
}
