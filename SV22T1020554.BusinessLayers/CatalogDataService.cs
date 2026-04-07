
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;

namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n danh m?c hàng hóa c?a h? th?ng, 
    /// bao g?m: m?t hàng (Product), thu?c tính c?a m?t hàng (ProductAttribute) và ?nh c?a m?t hàng (ProductPhoto).
    /// </summary>
    public static class CatalogDataService
    {
        private static readonly IProductRepository productDB;
        private static readonly IGenericRepository<Category> categoryDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static CatalogDataService()
        {
            categoryDB = new CategoryRepository(Configuration.ConnectionString);
            productDB = new ProductRepository(Configuration.ConnectionString);
        }

        #region Category

        /// <summary>
        /// T?m ki?m và l?y danh sách lo?i hàng dý?i d?ng phân trang.
        /// </summary>
        /// <param name="input">
        /// Thông tin t?m ki?m và phân trang (t? khóa t?m ki?m, trang c?n hi?n th?, s? d?ng m?i trang).
        /// </param>
        /// <returns>
        /// K?t qu? t?m ki?m dý?i d?ng danh sách lo?i hàng có phân trang.
        /// </returns>
        public static async Task<PagedResult<Category>> ListCategoriesAsync(PaginationSearchInput input)
        {
            return await categoryDB.ListAsync(input);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t lo?i hàng d?a vào m? lo?i hàng.
        /// </summary>
        /// <param name="CategoryID">M? lo?i hàng c?n t?m.</param>
        /// <returns>
        /// Ð?i tý?ng Category n?u t?m th?y, ngý?c l?i tr? v? null.
        /// </returns>
        public static async Task<Category?> GetCategoryAsync(int CategoryID)
        {
            return await categoryDB.GetAsync(CategoryID);
        }

        /// <summary>
        /// B? sung m?t lo?i hàng m?i vào h? th?ng.
        /// </summary>
        /// <param name="data">Thông tin lo?i hàng c?n b? sung.</param>
        /// <returns>M? lo?i hàng ðý?c t?o m?i.</returns>
        public static async Task<int> AddCategoryAsync(Category data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await categoryDB.AddAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t lo?i hàng.
        /// </summary>
        /// <param name="data">Thông tin lo?i hàng c?n c?p nh?t.</param>
        /// <returns>
        /// True n?u c?p nh?t thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> UpdateCategoryAsync(Category data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await categoryDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa m?t lo?i hàng d?a vào m? lo?i hàng.
        /// </summary>
        /// <param name="CategoryID">M? lo?i hàng c?n xóa.</param>
        /// <returns>
        /// True n?u xóa thành công, False n?u lo?i hàng ðang ðý?c s? d?ng
        /// ho?c vi?c xóa không th?c hi?n ðý?c.
        /// </returns>
        public static async Task<bool> DeleteCategoryAsync(int CategoryID)
        {
            if (await categoryDB.IsUsed(CategoryID))
                return false;

            return await categoryDB.DeleteAsync(CategoryID);
        }

        /// <summary>
        /// Ki?m tra xem m?t lo?i hàng có ðang ðý?c s? d?ng trong d? li?u hay không.
        /// </summary>
        /// <param name="CategoryID">M? lo?i hàng c?n ki?m tra.</param>
        /// <returns>
        /// True n?u lo?i hàng ðang ðý?c s? d?ng, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> IsUsedCategoryAsync(int CategoryID)
        {
            return await categoryDB.IsUsed(CategoryID);
        }

        #endregion

        #region Product

        /// <summary>
        /// T?m ki?m và l?y danh sách m?t hàng dý?i d?ng phân trang.
        /// </summary>
        /// <param name="input">
        /// Thông tin t?m ki?m và phân trang m?t hàng.
        /// </param>
        /// <returns>
        /// K?t qu? t?m ki?m dý?i d?ng danh sách m?t hàng có phân trang.
        /// </returns>
        public static async Task<PagedResult<Product>> ListProductsAsync(ProductSearchInput input)
        {
            return await productDB.ListAsync(input);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t m?t hàng.
        /// </summary>
        /// <param name="productID">M? m?t hàng c?n t?m.</param>
        /// <returns>
        /// Ð?i tý?ng Product n?u t?m th?y, ngý?c l?i tr? v? null.
        /// </returns>
        public static async Task<Product?> GetProductAsync(int productID)
        {
            return await productDB.GetAsync(productID);
        }

        /// <summary>
        /// B? sung m?t m?t hàng m?i vào h? th?ng.
        /// </summary>
        /// <param name="data">Thông tin m?t hàng c?n b? sung.</param>
        /// <returns>M? m?t hàng ðý?c t?o m?i.</returns>
        public static async Task<int> AddProductAsync(Product data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await productDB.AddAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t m?t hàng.
        /// </summary>
        /// <param name="data">Thông tin m?t hàng c?n c?p nh?t.</param>
        /// <returns>
        /// True n?u c?p nh?t thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> UpdateProductAsync(Product data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await productDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa m?t m?t hàng d?a vào m? m?t hàng.
        /// </summary>
        /// <param name="productID">M? m?t hàng c?n xóa.</param>
        /// <returns>
        /// True n?u xóa thành công, False n?u m?t hàng ðang ðý?c s? d?ng
        /// ho?c vi?c xóa không th?c hi?n ðý?c.
        /// </returns>
        public static async Task<bool> DeleteProductAsync(int productID)
        {
            if (await productDB.IsUsedAsync(productID))
                return false;

            return await productDB.DeleteAsync(productID);
        }

        /// <summary>
        /// Ki?m tra xem m?t m?t hàng có ðang ðý?c s? d?ng trong d? li?u hay không.
        /// </summary>
        /// <param name="productID">M? m?t hàng c?n ki?m tra.</param>
        /// <returns>
        /// True n?u m?t hàng ðang ðý?c s? d?ng, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> IsUsedProductAsync(int productID)
        {
            return await productDB.IsUsedAsync(productID);
        }

        #endregion

        #region ProductAttribute

        /// <summary>
        /// L?y danh sách các thu?c tính c?a m?t m?t hàng.
        /// </summary>
        /// <param name="productID">M? m?t hàng.</param>
        /// <returns>
        /// Danh sách các thu?c tính c?a m?t hàng.
        /// </returns>
        public static async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            return await productDB.ListAttributesAsync(productID);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t thu?c tính c?a m?t hàng.
        /// </summary>
        /// <param name="attributeID">M? thu?c tính.</param>
        /// <returns>
        /// Ð?i tý?ng ProductAttribute n?u t?m th?y, ngý?c l?i tr? v? null.
        /// </returns>
        public static async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            return await productDB.GetAttributeAsync(attributeID);
        }

        /// <summary>
        /// B? sung m?t thu?c tính m?i cho m?t hàng.
        /// </summary>
        /// <param name="data">Thông tin thu?c tính c?n b? sung.</param>
        /// <returns>M? thu?c tính ðý?c t?o m?i.</returns>
        public static async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await productDB.AddAttributeAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t thu?c tính m?t hàng.
        /// </summary>
        /// <param name="data">Thông tin thu?c tính c?n c?p nh?t.</param>
        /// <returns>
        /// True n?u c?p nh?t thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            return await productDB.UpdateAttributeAsync(data);
        }

        /// <summary>
        /// Xóa m?t thu?c tính c?a m?t hàng.
        /// </summary>
        /// <param name="attributeID">M? thu?c tính c?n xóa.</param>
        /// <returns>
        /// True n?u xóa thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            return await productDB.DeleteAttributeAsync(attributeID);
        }

        #endregion

        #region ProductPhoto

        /// <summary>
        /// L?y danh sách ?nh c?a m?t m?t hàng.
        /// </summary>
        /// <param name="productID">M? m?t hàng.</param>
        /// <returns>
        /// Danh sách ?nh c?a m?t hàng.
        /// </returns>
        public static async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            return await productDB.ListPhotosAsync(productID);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t ?nh c?a m?t hàng.
        /// </summary>
        /// <param name="photoID">M? ?nh.</param>
        /// <returns>
        /// Ð?i tý?ng ProductPhoto n?u t?m th?y, ngý?c l?i tr? v? null.
        /// </returns>
        public static async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            return await productDB.GetPhotoAsync(photoID);
        }

        /// <summary>
        /// B? sung m?t ?nh m?i cho m?t hàng.
        /// </summary>
        /// <param name="data">Thông tin ?nh c?n b? sung.</param>
        /// <returns>M? ?nh ðý?c t?o m?i.</returns>
        public static async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            return await productDB.AddPhotoAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t ?nh m?t hàng.
        /// </summary>
        /// <param name="data">Thông tin ?nh c?n c?p nh?t.</param>
        /// <returns>
        /// True n?u c?p nh?t thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            return await productDB.UpdatePhotoAsync(data);
        }

        /// <summary>
        /// Xóa m?t ?nh c?a m?t hàng.
        /// </summary>
        /// <param name="photoID">M? ?nh c?n xóa.</param>
        /// <returns>
        /// True n?u xóa thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> DeletePhotoAsync(long photoID)
        {
            return await productDB.DeletePhotoAsync(photoID);
        }

        #endregion
    }
}
