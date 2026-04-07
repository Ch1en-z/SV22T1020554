using SV22T1020554.Models.Common;

namespace SV22T1020554.DataLayers.Interfaces
{
    /// <summary>
    /// Ð?nh ngh?a các phép x? l? d? li?u ðõn gi?n trên m?t
    /// ki?u d? li?u T nào ðó (T là m?t Entity/DomainModel nào ðó)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Truy v?n, t?m ki?m d? li?u và tr? v? k?t qu? dý?i d?ng ðý?c phân trang
        /// </summary>
        /// <param name="input">Ð?u vào t?m ki?m, phân trang</param>
        /// <returns></returns>
        Task<PagedResult<T>> ListAsync(PaginationSearchInput input);

        /// <summary>
        /// L?y d? li?u c?a m?t b?n ghi có m? là id (tr? v? null n?u không có d? li?u)
        /// </summary>
        /// <param name="id">M? c?a d? li?u c?n l?y</param>
        /// <returns></returns>
        Task<T?> GetAsync(int id);

        /// <summary>
        /// B? sung m?t b?n ghi vào b?ng trong CSDL
        /// </summary>
        /// <param name="data">D? li?u c?n b? sung</param>
        /// <returns>M? c?a d?ng d? li?u ðý?c b? sung (thý?ng là IDENTITY)</returns>
        Task<int> AddAsync(T data);

        /// <summary>
        /// C?p nh?t m?t b?n ghi trong b?ng c?a CSDL
        /// </summary>
        /// <param name="data">D? li?u c?n c?p nh?t</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T data);

        /// <summary>
        /// Xóa b?n ghi có m? là id
        /// </summary>
        /// <param name="id">M? c?a b?n ghi c?n xóa</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Ki?m tra xem m?t b?n ghi có m? là id có d? li?u liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> IsUsed(int id);
    }
}
