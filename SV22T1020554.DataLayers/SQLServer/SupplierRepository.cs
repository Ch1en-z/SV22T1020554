using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.Models.Partner;
using SV22T1020554.Models.Common;
using SV22T1020554.DataLayers.Interfaces;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài ð?t các phép x? l? d? li?u cho nhà cung c?p trên SQL Server
    /// </summary>
    public class SupplierRepository : IGenericRepository<Supplier>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Kh?i t?o repository v?i chu?i k?t n?i
        /// </summary>
        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// B? sung m?t nhà cung c?p m?i vào cõ s? d? li?u
        /// </summary>
        public async Task<int> AddAsync(Supplier data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                            values (@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                            select @@identity;";

                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
                };

                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        /// <summary>
        /// Xóa m?t nhà cung c?p kh?i cõ s? d? li?u
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from Suppliers where SupplierID = @SupplierID";
                var parameters = new { SupplierID = id };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t nhà cung c?p
        /// </summary>
        public async Task<Supplier?> GetAsync(int id)
        {
            Supplier? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Suppliers where SupplierID = @SupplierID";
                var parameters = new { SupplierID = id };
                data = await connection.QueryFirstOrDefaultAsync<Supplier>(sql, parameters);
            }
            return data;
        }

        /// <summary>
        /// Ki?m tra xem nhà cung c?p hi?n t?i có d? li?u liên quan hay không
        /// </summary>
        public async Task<bool> IsUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Products where SupplierID = @SupplierID)
                                select 1
                            else 
                                select 0";
                var parameters = new { SupplierID = id };
                result = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return result;
        }

        /// <summary>
        /// T?m ki?m, ð?m s? lý?ng và l?y danh sách nhà cung c?p phân trang
        /// </summary>
        public async Task<PagedResult<Supplier>> ListAsync(PaginationSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var countSql = @"select count(*) from Suppliers 
                                 where (@searchValue = N'') or (SupplierName like @searchValue)";

                var listSql = @"select *
                                from (
                                    select *, row_number() over (order by SupplierName) as RowNumber
                                    from Suppliers 
                                    where (@searchValue = N'') or (SupplierName like @searchValue)
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
                var data = await connection.QueryAsync<Supplier>(listSql, parameters);

                return new PagedResult<Supplier>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t nhà cung c?p
        /// </summary>
        public async Task<bool> UpdateAsync(Supplier data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Suppliers 
                            set SupplierName = @SupplierName,
                                ContactName = @ContactName,
                                Province = @Province,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email
                            where SupplierID = @SupplierID";

                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    SupplierID = data.SupplierID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }
    }
}
