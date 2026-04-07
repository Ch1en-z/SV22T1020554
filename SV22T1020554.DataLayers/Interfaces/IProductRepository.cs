using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;

namespace SV22T1020554.DataLayers.Interfaces
{
    /// <summary>
    /// Ð?nh ngh?a các phép x? l? d? li?u cho m?t hàng
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// T?m ki?m và l?y danh sách m?t hàng dý?i d?ng phân trang
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResult<Product>> ListAsync(ProductSearchInput input);

        /// <summary>
        /// L?y thông tin 1 m?t hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Task<Product?> GetAsync(int productID);

        /// <summary>
        /// B? sung m?t hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns>M? m?t hàng ðý?c b? sung</returns>
        Task<int> AddAsync(Product data);

        /// <summary>
        /// C?p nh?t m?t hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Product data);

        /// <summary>
        /// Xóa m?t hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(int productID);

        /// <summary>
        /// Ki?m tra m?t hàng có d? li?u liên quan không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Task<bool> IsUsedAsync(int productID);

        /// <summary>
        /// L?y danh sách thu?c tính c?a m?t hàng
        /// </summary>
        /// <param name="productID">M? c?a m?t hàng</param>
        /// <returns></returns>
        Task<List<ProductAttribute>> ListAttributesAsync(int productID);

        /// <summary>
        /// L?y thông tin c?a m?t thu?c tính
        /// </summary>
        /// <param name="attributeID">M? c?a thu?c tính</param>
        /// <returns></returns>
        Task<ProductAttribute?> GetAttributeAsync(long attributeID);

        /// <summary>
        /// B? sung thu?c tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<long> AddAttributeAsync(ProductAttribute data);

        /// <summary>
        /// C?p nh?t thu?c tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> UpdateAttributeAsync(ProductAttribute data);

        /// <summary>
        /// Xóa thu?c tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        Task<bool> DeleteAttributeAsync(long attributeID);

        /// <summary>
        /// L?y danh sách ?nh c?a m?t hàng
        /// </summary>
        /// <param name="productID">M? m?t hàng</param>
        /// <returns></returns>
        Task<List<ProductPhoto>> ListPhotosAsync(int productID);

        /// <summary>
        /// L?y thông tin 1 ?nh c?a m?t hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        Task<ProductPhoto?> GetPhotoAsync(long photoID);

        /// <summary>
        /// B? sung ?nh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<long> AddPhotoAsync(ProductPhoto data);

        /// <summary>
        /// C?p nh?t ?nh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> UpdatePhotoAsync(ProductPhoto data);

        /// <summary>
        /// Xóa ?nh
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        Task<bool> DeletePhotoAsync(long photoID);
    }
}
