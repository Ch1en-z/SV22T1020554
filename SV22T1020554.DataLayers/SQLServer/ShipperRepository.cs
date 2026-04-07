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
    /// Cài ð?t các phép x? l? d? li?u cho ngý?i giao hàng trên SQL Server
    /// </summary>
    public class ShipperRepository : IGenericRepository<Shipper>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Kh?i t?o repository v?i chu?i k?t n?i
        /// </summary>
        public ShipperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// B? sung m?t ngý?i giao hàng m?i vào cõ s? d? li?u
        /// </summary>
        public async Task<int> AddAsync(Shipper data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Shippers(ShipperName, Phone)
                            values (@ShipperName, @Phone);
                            select @@identity;";

                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? ""
                };

                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        /// <summary>
        /// Xóa m?t ngý?i giao hàng kh?i cõ s? d? li?u
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from Shippers where ShipperID = @ShipperID";
                var parameters = new { ShipperID = id };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t ngý?i giao hàng
        /// </summary>
        public async Task<Shipper?> GetAsync(int id)
        {
            Shipper? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Shippers where ShipperID = @ShipperID";
                var parameters = new { ShipperID = id };
                data = await connection.QueryFirstOrDefaultAsync<Shipper>(sql, parameters);
            }
            return data;
        }

        /// <summary>
        /// Ki?m tra xem ngý?i giao hàng hi?n t?i có d? li?u liên quan (ðõn hàng) hay không
        /// </summary>
        public async Task<bool> IsUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Orders where ShipperID = @ShipperID)
                                select 1
                            else 
                                select 0";
                var parameters = new { ShipperID = id };
                result = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return result;
        }

        /// <summary>
        /// T?m ki?m, ð?m s? lý?ng và l?y danh sách ngý?i giao hàng phân trang
        /// </summary>
        public async Task<PagedResult<Shipper>> ListAsync(PaginationSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var countSql = @"select count(*) from Shippers 
                                 where (@searchValue = N'') or (ShipperName like @searchValue)";

                var listSql = @"select *
                                from (
                                    select *, row_number() over (order by ShipperName) as RowNumber
                                    from Shippers 
                                    where (@searchValue = N'') or (ShipperName like @searchValue)
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
                var data = await connection.QueryAsync<Shipper>(listSql, parameters);

                return new PagedResult<Shipper>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t ngý?i giao hàng
        /// </summary>
        public async Task<bool> UpdateAsync(Shipper data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Shippers 
                            set ShipperName = @ShipperName,
                                Phone = @Phone
                            where ShipperID = @ShipperID";

                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                    ShipperID = data.ShipperID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }
    }
}
