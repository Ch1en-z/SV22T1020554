using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;
using SV22T1020554.DataLayers.Interfaces;

// Đảm bảo không bị xung đột tên Product
using Product = SV22T1020554.Models.Catalog.Product;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho mặt hàng, ảnh và thuộc tính mặt hàng trên SQL Server
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo repository với chuỗi kết nối
        /// </summary>
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Xử lý dữ liệu Mặt hàng (Product)

        public async Task<int> AddAsync(Product data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                            values (@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                            select @@identity;";

                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };

                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        public async Task<bool> DeleteAsync(int productID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from ProductPhotos where ProductID = @ProductID;
                            delete from ProductAttributes where ProductID = @ProductID;
                            delete from Products where ProductID = @ProductID;";

                var parameters = new { ProductID = productID };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<Product?> GetAsync(int productID)
        {
            Product? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Products where ProductID = @ProductID";
                var parameters = new { ProductID = productID };
                data = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);
            }
            return data;
        }

        public async Task<bool> IsUsedAsync(int productID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new { ProductID = productID };
                result = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return result;
        }

        public async Task<PagedResult<Product>> ListAsync(ProductSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var countSql = @"select count(*) from Products 
                                 where (@searchValue = N'') or (ProductName like @searchValue)";

                // Ta có thể bổ sung lọc theo CategoryID, SupplierID, v.v... ở đây nếu cần cho trang quản trị
                // Nhưng riêng với Tìm kiếm hàng để bán (Order), ta quan tâm IsSelling = 1
                
                var listSql = @"select *
                                from (
                                    select *, row_number() over (order by ProductName) as RowNumber
                                    from Products 
                                    where ((@searchValue = N'') or (ProductName like @searchValue))
                                      and (@categoryID = 0 or CategoryID = @categoryID)
                                      and (@supplierID = 0 or SupplierID = @supplierID)
                                      and (Price >= @minPrice)
                                      and (@maxPrice <= 0 or Price <= @maxPrice)
                                ) as t
                                where (@pageSize = 0) 
                                   or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber";

                var parameters = new
                {
                    page = input.Page,
                    pageSize = input.PageSize,
                    searchValue = searchValue,
                    categoryID = input.CategoryID,
                    supplierID = input.SupplierID,
                    minPrice = input.MinPrice,
                    maxPrice = input.MaxPrice
                };

                int rowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue = searchValue });
                var data = await connection.QueryAsync<Product>(listSql, parameters);

                return new PagedResult<Product>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        public async Task<bool> UpdateAsync(Product data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Products 
                            set ProductName = @ProductName,
                                ProductDescription = @ProductDescription,
                                SupplierID = @SupplierID,
                                CategoryID = @CategoryID,
                                Unit = @Unit,
                                Price = @Price,
                                Photo = @Photo,
                                IsSelling = @IsSelling
                            where ProductID = @ProductID";

                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                    ProductID = data.ProductID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        #endregion

        #region Xử lý dữ liệu Ảnh mặt hàng (ProductPhoto)

        public async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            long id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            select @@identity;";

                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };

                id = await connection.ExecuteScalarAsync<long>(sql, parameters);
            }
            return id;
        }

        public async Task<bool> DeletePhotoAsync(long photoID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                data = await connection.QueryFirstOrDefaultAsync<ProductPhoto>(sql, parameters);
            }
            return data;
        }

        public async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from ProductPhotos where ProductID = @ProductID order by DisplayOrder";
                var parameters = new { ProductID = productID };
                var data = await connection.QueryAsync<ProductPhoto>(sql, parameters);
                return data.ToList();
            }
        }

        public async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update ProductPhotos 
                            set ProductID = @ProductID,
                                Photo = @Photo,
                                Description = @Description,
                                DisplayOrder = @DisplayOrder,
                                IsHidden = @IsHidden
                            where PhotoID = @PhotoID";

                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                    PhotoID = data.PhotoID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        #endregion

        #region Xử lý dữ liệu Thuộc tính mặt hàng (ProductAttribute)

        public async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            long id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            select @@identity;";

                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };

                id = await connection.ExecuteScalarAsync<long>(sql, parameters);
            }
            return id;
        }

        public async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                data = await connection.QueryFirstOrDefaultAsync<ProductAttribute>(sql, parameters);
            }
            return data;
        }

        public async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder";
                var parameters = new { ProductID = productID };
                var data = await connection.QueryAsync<ProductAttribute>(sql, parameters);
                return data.ToList();
            }
        }

        public async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update ProductAttributes 
                            set ProductID = @ProductID,
                                AttributeName = @AttributeName,
                                AttributeValue = @AttributeValue,
                                DisplayOrder = @DisplayOrder
                            where AttributeID = @AttributeID";

                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                    AttributeID = data.AttributeID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        #endregion
    }
}
