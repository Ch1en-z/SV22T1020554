using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;
using SV22T1020554.DataLayers.Interfaces;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài ð?t các phép x? l? d? li?u cho lo?i hàng trên SQL Server
    /// </summary>
    public class CategoryRepository : IGenericRepository<Category>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Kh?i t?o repository v?i chu?i k?t n?i
        /// </summary>
        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// B? sung m?t lo?i hàng m?i vào cõ s? d? li?u
        /// </summary>
        public async Task<int> AddAsync(Category data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Categories(CategoryName, Description)
                            values (@CategoryName, @Description);
                            select @@identity;";

                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? ""
                };

                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        /// <summary>
        /// Xóa m?t lo?i hàng kh?i cõ s? d? li?u
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from Categories where CategoryID = @CategoryID";
                var parameters = new { CategoryID = id };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t lo?i hàng
        /// </summary>
        public async Task<Category?> GetAsync(int id)
        {
            Category? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Categories where CategoryID = @CategoryID";
                var parameters = new { CategoryID = id };
                data = await connection.QueryFirstOrDefaultAsync<Category>(sql, parameters);
            }
            return data;
        }

        /// <summary>
        /// Ki?m tra xem lo?i hàng hi?n t?i có d? li?u liên quan (m?t hàng) hay không
        /// </summary>
        public async Task<bool> IsUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Products where CategoryID = @CategoryID)
                                select 1
                            else 
                                select 0";
                var parameters = new { CategoryID = id };
                result = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return result;
        }

        /// <summary>
        /// T?m ki?m, ð?m s? lý?ng và l?y danh sách lo?i hàng phân trang
        /// </summary>
        public async Task<PagedResult<Category>> ListAsync(PaginationSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var countSql = @"select count(*) from Categories 
                                 where (@searchValue = N'') or (CategoryName like @searchValue)";

                var listSql = @"select *
                                from (
                                    select *, row_number() over (order by CategoryName) as RowNumber
                                    from Categories 
                                    where (@searchValue = N'') or (CategoryName like @searchValue)
                                ) as t
                                where (@pageSize = 0) 
                                   or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber";

                var parameters = new
                {
                    page = input.Page,
                    pageSize = input.PageSize,
                    searchValue = searchValue
                };

                int rowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue = searchValue });
                var data = await connection.QueryAsync<Category>(listSql, parameters);

                return new PagedResult<Category>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t lo?i hàng
        /// </summary>
        public async Task<bool> UpdateAsync(Category data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Categories 
                            set CategoryName = @CategoryName,
                                Description = @Description
                            where CategoryID = @CategoryID";

                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
                    CategoryID = data.CategoryID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }
    }
}
